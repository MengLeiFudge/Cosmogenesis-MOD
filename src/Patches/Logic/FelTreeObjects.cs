using GalacticScale;
using HarmonyLib;
using ProjectGenesis.Patches.UI.Utils;
using ProjectGenesis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectGenesis.Patches.Logic
{
    [HarmonyPatch]
    internal class FelTreeObjects
    {
        private static int patchMiningTick = 0;
        //开采石头时拦截
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerAction_Mine), "GameTick")]
        public static void GameTickPatch(ref PlayerAction_Mine __instance)
        {
            double num = 0.01666666753590107;
            PlanetFactory factory = __instance.player.factory;
            __instance.miningFlag = 0;
            __instance.veinMiningFlag = 0;
            PlanetData localPlanet = GameMain.localPlanet;
            if (factory != null && localPlanet != null && localPlanet.type == EPlanetType.Gas && __instance.player.movementState <= EMovementState.Fly && __instance.player.isAlive)
            {
                return;
            }


            if (factory == null)
            {
                __instance.miningId = 0;
            }
            else
            {
                if (__instance.controller.gameData.guideRunning)
                {
                    return;
                }

                double reactorPowerConsRatio = __instance.player.mecha.reactorPowerConsRatio;
                double num6 = __instance.player.mecha.miningPower * reactorPowerConsRatio * num;
                __instance.player.mecha.QueryEnergy(num6, out var energyGet, out var ratio);
                int num7 = (int)(__instance.player.mecha.miningSpeed * ratio * 10000f + 0.49f);
                EObjectType eObjectType = EObjectType.None;
                int num8 = 0;
                OrderNode currentOrder = __instance.player.currentOrder;
                if (currentOrder != null && currentOrder.type == EOrderType.Mine && currentOrder.targetReached)
                {
                    eObjectType = currentOrder.objType;
                    num8 = currentOrder.objId;
                }

                if (eObjectType == EObjectType.None)
                {
                    return;
                }

                if (eObjectType == EObjectType.Vegetable && factory.GetVegeData(num8).id == 0)
                {
                    return;
                }

                if (eObjectType == EObjectType.Vein)
                {
                    return;
                }

                if (__instance.miningType != eObjectType || __instance.miningId != num8)
                {
                    __instance.miningType = eObjectType;
                    __instance.miningId = num8;
                    patchMiningTick = 0;
                }

                if (__instance.miningId != 0)
                {
                    if (__instance.miningType == EObjectType.Vegetable)
                    {
                        VegeData vegeData = factory.GetVegeData(__instance.miningId);
                        __instance.miningProtoId = vegeData.protoId;
                        VegeProto vegeProto = LDB.veges.Select(vegeData.protoId);
                        if (vegeProto != null)
                        {
                            patchMiningTick += num7;
                            __instance.player.mecha.coreEnergy -= energyGet;
                            __instance.player.mecha.MarkEnergyChange(5, 0.0 - num6);
                            __instance.percent = Mathf.Clamp01((float)((double)patchMiningTick / (double)(vegeProto.MiningTime * 10000)));
                            if (patchMiningTick >= vegeProto.MiningTime * 10000)
                            {
                                OnMine(ref factory, vegeData.id);
                                patchMiningTick = 0;
                            }
                        }else
                        {
                            OnMine(ref factory, vegeData.id);
                            patchMiningTick = 0;
                        }
                    }
                }
            }
        }

        public static void OnMine(ref PlanetFactory __instance, int id)
        {
            //参数合法性校验
            if (__instance.vegePool[id].id == 0) { return; }
            int ItemProtoID = 0;
            int ItemCount = 0;
            DotNet35Random DotNet35Random = new DotNet35Random();
            double Random = DotNet35Random.NextDouble();
            //校验是否为指定离散矿石类型
            if (IsStone(__instance.vegePool[id].protoId) == false)
            {
                if (LDB.items.Select(ProtoID.I激素生长菌群) == null || LDB.items.Select(ProtoID.I高速生长菌群) == null || LDB.items.Select(6245) == null || LDB.items.Select(6252) == null)
                {
                    return;
                }
                if (__instance.planet.theme == 8 || __instance.planet.theme == 25)
                {
                    //0.0500概率获得高速生长菌群
                    if (Random > 0.00001 && Random <= 0.05001)
                    {
                        ItemProtoID = ProtoID.I高速生长菌群;
                        ItemCount = 1;
                    }
                    else if (Random > 0.05001 && Random <= 0.45001)
                    {
                        ItemProtoID = ProtoID.I激素生长菌群;
                        ItemCount = 2;
                    }
                } else if (__instance.planet.theme == 14)
                {
                    if (Random > 0.00001 && Random <= 0.05001)
                    {
                        ItemProtoID = 6245;
                        ItemCount = 1;
                    }
                    else if (Random > 0.05001 && Random <= 0.65001)
                    {
                        ItemProtoID = 6252;
                        ItemCount = 2;
                    }
                }
                else
                {
                    //0.200概率获得激素生长菌群
                    if (Random > 0.00001 && Random <= 0.25001)
                    {
                        ItemProtoID = ProtoID.I激素生长菌群;
                        ItemCount = 1;
                    }
                }

                if (ItemProtoID < 1)
                {
                    //Util.Log("未获得道具，随机数：" + Random.ToString());
                    return;
                }
                //Util.Log("恒星ID：" + __instance.planet.star.id.ToString() + "  行星ID：" + __instance.planet.id.ToString() + "  对象ID：" + id.ToString() + "  对象类型：" + __instance.vegePool[id].protoId.ToString());

            }
            else
            {
                if (LDB.items.Select(6216) == null || LDB.items.Select(6215) == null || LDB.items.Select(6244) == null)
                {
                    return;
                }
                if (__instance.planet.star.type == EStarType.NeutronStar)
                {
                    if (Random > 0.00001 && Random <= 0.1501)
                    {
                        ItemProtoID = 6244;
                        ItemCount = 1;
                    }
                }
                else
                {
                    if (__instance.planet.theme == 17)
                    {
                        //0.20概率获得未知射线遗留样本
                        if (Random > 0.00001 && Random <= 0.4001)
                        {
                            ItemProtoID = 6216;
                            ItemCount = 1;
                        }
                    }
                    else if (__instance.planet.theme == 6)
                    {
                        if (Random > 0.00001 && Random <= 0.3001)
                        {
                            ItemProtoID = 6215;
                            ItemCount = 1;
                        }
                    }
                }
                if (ItemProtoID < 1)
                {
                    //Util.Log("未获得道具，随机数：" + Random.ToString());
                    return;
                }
            }
            GainTechAwards(ItemProtoID, ItemCount);
            return;
        }


        ///<summary>
        ///添加物品奖励
        ///</summary>
        public static void GainTechAwards(int itemId, int count)
        {
            int package = GameMain.mainPlayer.TryAddItemToPackage(itemId, count, count, true);
            if (package < count)
            {
                UIRealtimeTip.Popup("无法获得科技奖励".Translate());
                //Util.Log("物品添加失败");
                //Util.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "无法获得科技奖励");
            }
            else
            {
                UIItemup.Up(itemId, count);
                UIRealtimeTip.Popup("?!!".Translate());
                //Util.Log("添加物品至背包：" + itemId.ToString());
                //Util.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "添加物品至背包");
            }

        }

        public static bool IsStone(int protoID)
        {
            if (protoID >= 1 && protoID <= 12)
            {
                return true;
            }
            else if (protoID >= 19 && protoID <= 21)
            {
                return true;
            }
            else if (protoID >= 48 && protoID <= 59)
            {
                return true;
            }
            else if (protoID >= 66 && protoID <= 90)
            {
                return true;
            }
            else if (protoID >= 601 && protoID <= 605)
            {
                return true;
            }
            else if (protoID >= 611 && protoID <= 734)
            {
                return true;
            }
            else if (protoID >= 1041 && protoID <= 1044)
            {
                return true;
            }
            else if (protoID >= 1051 && protoID <= 1055)
            {
                return true;
            }
            else if (protoID >= 1061 && protoID <= 1066)
            {
                return true;
            }
            else if (protoID >= 1071 && protoID <= 1074)
            {
                return true;
            }
            return false;
        }

    }
}
