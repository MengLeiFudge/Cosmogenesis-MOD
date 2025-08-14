using HarmonyLib;
using ProjectGenesis.Patches.Logic.AddVein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using static UIPlayerDeliveryPanel;
using CommonAPI;
using ProjectGenesis.Utils;
using MoreMegaStructure;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Playables;

namespace ProjectGenesis.Patches.Logic
{
    internal class StarGate
    {
        private static List<int> starGateList = new List<int>();
        //StationComponent  DetermineDispatch
        public static void RecordStarGate(int itemId, int plantId)
        {
            if (itemId == 6281)
            {
                int starIndex = plantId / 100;
                starGateList.Add(starIndex);
            }
        }

        public static void DismantleStarGate(int itemId, int plantId)
        {
            if (itemId == 6281)
            {
                int starIndex = plantId / 100;
                starGateList.Remove(starIndex);
            }
        }

        public static bool StarGateExit(int plantId)
        {
            int starIndex = plantId / 100;
            return starGateList.Contains(starIndex);
        }


        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DetermineDispatch))]
        [HarmonyPrefix]
        public static void DetermineDispatchPrefixPatch(ref StationComponent __instance)
        {
            // 有中继器航线可以免除翘曲器跨星系物流，为了绕过翘曲器小于两个就跳过的逻辑，先加20个翘曲器，函数结束时再回收
            __instance.warperCount += 20;
        }

        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DetermineDispatch))]
        [HarmonyPostfix]
        public static void DetermineDispatchPostfixPatch(ref StationComponent __instance)
        {
            __instance.warperCount -= 20;
        }

        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DispatchSupplyShip))]
        [HarmonyPrefix]
        public static bool DispatchSupplyShipPrefixPatch(ref StationComponent __instance, bool takeWarper, StationComponent other, bool __result)
        {
            // 如果本身不需要翘曲器，说明是星系内物流，直接返回不拦截
            if (!takeWarper)
            {
                return true;
            }
            if (StarGateExit(__instance.planetId) && StarGateExit(other.planetId))
            {
                int num2 = __instance.QueryIdleShip(__instance.nextShipIndex);
                if (num2 >= 0)
                {
                    // 如果是星际物流，且两端都有中继器，则增加两个翘曲器以供消耗，达成表面不消耗翘曲器的逻辑，不拦截，处理原本发船逻辑
                    __instance.warperCount += 2;
                    return true;
                }
            }

            if (__instance.energyMax != 12000000000)
            {
                // 没有中继器，又不是深空物流港，不允许翘曲，直接拦截，返回false
                __result = false;
                return false;
            }
            if (__instance.warperCount <= 21)
            {
                // 没有中继器，是深空物流港，但翘曲器小于21，说明原本翘曲器只有1或者0，不允许翘曲，直接拦截，返回false
                __result = false;
                return false;
            }
            // 没有中继器，是深空物流港，且翘曲器大于1，允许翘曲，不拦截，处理原本发船逻辑
            __result = false;
            return true;
        }



        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DispatchDemandShip))]
        [HarmonyPrefix]
        public static bool DispatchDemandShipPrefixPatch(ref StationComponent __instance, bool takeWarper, StationComponent other, bool __result)
        {
            if (!takeWarper)
            {
                return true;
            }
            if (StarGateExit(__instance.planetId) && StarGateExit(other.planetId))
            {
                int num = __instance.QueryIdleShip(__instance.nextShipIndex);
                if (num >= 0)
                {
                    __instance.warperCount += 2;
                    return true;
                }
            }

            if (__instance.energyMax != 12000000000)
            {
                __result = false;
                return false;
            }
            if (__instance.warperCount < 21)
            {
                __result = false;
                return false;
            }
            __result = false;
            return true;
        }



        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DetermineFramingDispatchTime))]
        [HarmonyPrefix]
        public static bool DetermineFramingDispatchTimePrefixPatch(ref bool __result)
        {
            __result = true;
            return false;
        }


        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.DetermineDispatch))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StationComponent_DetermineDispatch_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(false, new CodeMatch(OpCodes.Add), new CodeMatch(OpCodes.Ldloc_S));
            object V_29 = matcher.Advance(2).Operand; // 变量索引

            matcher.MatchForward(false, new CodeMatch(OpCodes.Stloc_S), new CodeMatch(OpCodes.Ldc_I4_0),
                 new CodeMatch(OpCodes.Stloc_S), new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldfld));
            object V_34 = matcher.Operand; // 变量索引

            matcher.Advance(1).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, V_29),
                new CodeInstruction(OpCodes.Ldloca_S, V_34),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StarGate), nameof(StarGateEnergyReduction),
                new Type[] {
                    typeof(StationComponent),
                    typeof(StationComponent),
                    typeof(long).MakeByRefType(),
                }
            )));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_0), new CodeMatch(OpCodes.Call));
            object V_42 = matcher.Advance(2).Operand; // 变量索引
            object V_38 = matcher.Advance(1).Operand; // 变量索引

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, V_38),
                new CodeInstruction(OpCodes.Ldloca_S, V_42),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StarGate), nameof(StarGateEnergyReduction),
                new Type[] {
                    typeof(StationComponent),
                    typeof(StationComponent),
                    typeof(long).MakeByRefType()
                }
            )));

            return matcher.InstructionEnumeration();
        }
        public static void StarGateEnergyReduction(StationComponent thisStationComponent, StationComponent otherStationComponent, ref long energy)
        {
            if (ProjectGenesis.MoreMegaStructureCompatibility)
            {
                int thisStarIndex = thisStationComponent.planetId / 100;
                Type targetType = AccessTools.TypeByName("MoreMegaStructure.WarpArray");
                if (targetType == null) return;

                FieldInfo starIsInWhichWarpArrayField = AccessTools.Field(targetType, "starIsInWhichWarpArray");
                if (starIsInWhichWarpArrayField == null) return;
                int[] starIsInWhichWarpArray = (int[])starIsInWhichWarpArrayField.GetValue(null);

                FieldInfo tripEnergyCostRatioByStarIndexField = AccessTools.Field(targetType, "tripEnergyCostRatioByStarIndex");
                if (tripEnergyCostRatioByStarIndexField == null) return;
                double[] tripEnergyCostRatioByStarIndex = (double[])tripEnergyCostRatioByStarIndexField.GetValue(null);

                if (starIsInWhichWarpArray[thisStarIndex] >= 0)
                {
                    if (tripEnergyCostRatioByStarIndex[thisStarIndex] > 0.2)
                    {
                        if (StarGateExit(thisStationComponent.planetId) && StarGateExit(otherStationComponent.planetId))
                        {
                            energy = (long)((energy / tripEnergyCostRatioByStarIndex[thisStarIndex]) * 0.2); // 中继器航线能量消耗降低为原来的20%
                        }
                    }
                    return;
                }
            }
            if (StarGateExit(thisStationComponent.planetId) && StarGateExit(otherStationComponent.planetId))
            {
                energy = (long)(energy * 0.2); // 中继器航线能量消耗降低为原来的20%
            }
        }



        internal static void Export(BinaryWriter w)
        {
            w.Write(starGateList.Count);
            foreach (var item in starGateList)
            {
                w.Write(item); // 逐个写入元素
            }
        }

        internal static void Import(BinaryReader r)
        {
            try
            {
                int count = 0;
                count = r.ReadInt32(); // 先读取元素数量
                for (int i = 1; i < count; i++)
                {
                    starGateList.Add(r.ReadInt32()); // 逐个读取元素
                }
            }
            catch (EndOfStreamException)
            {
                // ignored
            }
        }
    }
}
