using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGenesis.Patches.Logic.MathematicalRateEngine
{
    internal class SiloAndEjector
    {
        /// <summary>
        /// 火箭发射器所需火箭修正，注意如果更改了巨构类型，而发射器内还存有不相符的火箭，该火箭将直接消失（为了防止用廉价火箭白嫖高价火箭）
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static void SiloUpdatePatch(ref SiloComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2312) return; //只修改原始火箭发射器

            if (starIndex < 0 || starIndex > 999)
            {
                //Debug.LogWarning("SiloInternalUpdate Patch Error because starIndex out of range.");
                return;
            }

            int bulletIdExpected = 1503;

            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole)
            {
                if (ProjectGenesis.MoreMegaStructureCompatibility)
                {
                    Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
                    if (targetType == null) return;

                    FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
                    if (StarMegaStructureTypeField == null) return;
                    int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

                    if (StarMegaStructureType[starIndex] != 0)
                    {
                        if (__instance.bulletId == 6228 || __instance.bulletId == 6504 || __instance.bulletId == 6502)
                        {
                            __instance.bulletCount = 0;
                            __instance.bulletInc = 0;
                            __instance.bulletId = 1503;
                        }
                        return;
                    }
                }
                if (!GameMain.history.TechUnlocked(1952))
                {
                    bulletIdExpected = 6228;
                }
                else if (!GameMain.history.TechUnlocked(1960))
                {
                    bulletIdExpected = 6504;
                }
                else
                {
                    bulletIdExpected = 6502;
                }
            }
            
            if (__instance.bulletId != bulletIdExpected)
            {
                __instance.bulletCount = 0;
                __instance.bulletInc = 0;
                __instance.bulletId = bulletIdExpected;
            }
            
        }

        /// <summary>
        /// 弹射器所需发射物修正，类似上面的发射井
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static void EjectorUpdatePatch(ref EjectorComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2311) return; //只修改原始弹射器

            if (starIndex < 0 || starIndex > 999)
            {
                return;
            }
            int bulletIdExpected = 1501;
            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole)
            {
                if (ProjectGenesis.MoreMegaStructureCompatibility)
                {
                    Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
                    if (targetType == null) return;

                    FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
                    if (StarMegaStructureTypeField == null) return;
                    int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

                    if (StarMegaStructureType[starIndex] != 0)
                    {
                        if (StarMegaStructureType[starIndex] == 2)
                        {
                            if (__instance.bulletId != 6006)
                            {
                                __instance.bulletCount = 0;
                                __instance.bulletInc = 0;
                                __instance.bulletId = 6006;
                            }
                        }
                        else
                        {
                            if (__instance.bulletId != 1501)
                            {
                                __instance.bulletCount = 0;
                                __instance.bulletInc = 0;
                                __instance.bulletId = 1501;
                            }
                        }
                        return;
                    }
                }
                if (!GameMain.history.TechUnlocked(1802))
                {
                    bulletIdExpected = 6228;
                }
                else if (!GameMain.history.TechUnlocked(1952))
                {
                    bulletIdExpected = 9480;
                }
                else if (!GameMain.history.TechUnlocked(1960))
                {
                    bulletIdExpected = 1803;
                }
                else
                {
                    bulletIdExpected = 9482;
                }
            }
            if (__instance.bulletId != bulletIdExpected)
            {
                __instance.bulletCount = 0;
                __instance.bulletInc = 0;
                __instance.bulletId = bulletIdExpected;
            }
            
        }
    }
}
