using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using static GalacticScale.PatchOnUIGalaxySelect;
using static System.Collections.Specialized.BitVector32;

namespace ProjectGenesis.Patches.Logic.CurvatureLogistics
{
    internal class CurvatureLogistics
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnPlayerIntendToTransferItems")]
        public static bool OnPlayerIntendToTransferItemsPatch(UIControlPanelStationInspector __instance, int _itemId, int _itemCount, int _itemInc)
        {
            if (__instance.stationId == 0 || __instance.factory == null)
            {
                return false;
            }
            StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
            if (stationComponent == null || stationComponent.id != __instance.stationId)
            {
                return false;
            }
            if (_itemId == 5001)
            {
                if (__instance.droneIconButton.button.interactable)
                {
                    __instance.OnDroneIconClick(_itemId);
                    return false;
                }
            }
            else if (stationComponent.isStellar && (_itemId == 5002 || _itemId == 6230))
            {
                Debug.LogFormat("scppppppp114514 _itemId {0}", _itemId);
                if (__instance.shipIconButton.button.interactable)
                {
                    __instance.OnShipIconClick(_itemId);
                    return false;
                }
            }
            else if (stationComponent.isStellar && _itemId == 1210 && __instance.warperIconButton.button.interactable)
            {
                __instance.OnWarperIconClick(_itemId);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnShipIconClick")]
        public static bool OnShipIconClickPatch(UIControlPanelStationInspector __instance, int obj)
        {
            if (__instance.stationId == 0 || __instance.factory == null)
            {
                return false;
            }
            StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
            if (stationComponent == null || stationComponent.id != __instance.stationId)
            {
                return false;
            }
            if (!stationComponent.isStellar)
            {
                return false;
            }
            if (stationComponent.isCollector || stationComponent.isVeinCollector)
            {
                return false;
            }
            if (__instance.player.inhandItemId > 0 && __instance.player.inhandItemCount == 0)
            {
                __instance.player.SetHandItems(0, 0, 0);
                return false;
            }
            if (__instance.player.inhandItemId > 0 && __instance.player.inhandItemCount > 0)
            {
                Debug.LogFormat("scppppppppppp112 inhandItemId {0}", __instance.player.inhandItemId);
                int num = 5002;
                if (__instance.station.energyMax == 12000000000)
                {
                    num = 6230;
                }

                ItemProto itemProto = LDB.items.Select(num);
                if (__instance.player.inhandItemId != num)
                {
                    UIRealtimeTip.Popup("只能放入".Translate() + itemProto.name, true, 0);
                    return false;
                }
                ItemProto itemProto2 = LDB.items.Select((int)__instance.factory.entityPool[stationComponent.entityId].protoId);
                int num2 = (itemProto2 != null) ? itemProto2.prefabDesc.stationMaxShipCount : 10;
                int num3 = stationComponent.idleShipCount + stationComponent.workShipCount;
                int num4 = num2 - num3;
                if (num4 < 0)
                {
                    num4 = 0;
                }
                int num5 = (__instance.player.inhandItemCount < num4) ? __instance.player.inhandItemCount : num4;
                if (num5 <= 0)
                {
                    UIRealtimeTip.Popup("栏位已满".Translate(), true, 0);
                    return false;
                }
                int inhandItemCount = __instance.player.inhandItemCount;
                int inhandItemInc = __instance.player.inhandItemInc;
                int num6 = num5;
                int num7 = __instance.split_inc(ref inhandItemCount, ref inhandItemInc, num6);
                stationComponent.idleShipCount += num6;
                __instance.player.AddHandItemCount_Unsafe(-num6);
                __instance.player.SetHandItemInc_Unsafe(__instance.player.inhandItemInc - num7);
                if (__instance.player.inhandItemCount <= 0)
                {
                    __instance.player.SetHandItemId_Unsafe(0);
                    __instance.player.SetHandItemCount_Unsafe(0);
                    __instance.player.SetHandItemInc_Unsafe(0);
                    return false;
                }
            }
            else if (__instance.player.inhandItemId == 0 && __instance.player.inhandItemCount == 0)
            {
                if (!__instance.isLocal)
                {
                    UIRealtimeTip.Popup("非本地星球拿取提示".Translate(), true, 0);
                    return false;
                }
                int idleShipCount = stationComponent.idleShipCount;
                if (idleShipCount <= 0)
                {
                    return false;
                }
                if (VFInput.shift || VFInput.control)
                {
                    if (__instance.station.energyMax == 12000000000)
                    {
                        int upCount = __instance.player.TryAddItemToPackage(6230, idleShipCount, 0, false, 0, false);
                        UIItemup.Up(6230, upCount);
                    }
                    else
                    {
                        int upCount = __instance.player.TryAddItemToPackage(5002, idleShipCount, 0, false, 0, false);
                        UIItemup.Up(5002, upCount);
                    }
                }
                else
                {
                    if (__instance.station.energyMax == 12000000000)
                    {
                        __instance.player.SetHandItemId_Unsafe(6230);
                    }
                    else
                    {
                        __instance.player.SetHandItemId_Unsafe(5002);
                    }
                    __instance.player.SetHandItemCount_Unsafe(idleShipCount);
                    __instance.player.SetHandItemInc_Unsafe(0);
                }
                stationComponent.idleShipCount = 0;
            }
            return false;
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStationWindow), "OnPlayerIntendToTransferItems")]
        public static bool OnPlayerIntendToTransferItemsPatch(UIStationWindow __instance, int _itemId, int _itemCount, int _itemInc)
        {
            if (__instance.stationId == 0 || __instance.factory == null)
            {
                return false;
            }

            StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
            if (stationComponent == null || stationComponent.id != __instance.stationId)
            {
                return false;
            }

            if (_itemId == 5001)
            {
                if (__instance.droneIconButton.button.interactable)
                {
                    __instance.OnDroneIconClick(_itemId);
                }
            }
            else if (stationComponent.isStellar && (_itemId == 5002 || _itemId == 6230))
            {
                
                if (__instance.shipIconButton.button.interactable)
                {
                    Debug.LogFormat("scppppppp1145141919 _itemId {0}", _itemId);
                    __instance.OnShipIconClick(_itemId);
                }
            }
            else if (stationComponent.isStellar && _itemId == 1210 && __instance.warperIconButton.button.interactable)
            {
                __instance.OnWarperIconClick(_itemId);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStationWindow), "OnShipIconClick")]
        public static bool OnShipIconClickPatch(UIStationWindow __instance, int obj)
        {
            if (__instance.stationId == 0 || __instance.factory == null)
            {
                return false;
            }

            StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
            if (stationComponent == null || stationComponent.id != __instance.stationId || !stationComponent.isStellar || stationComponent.isCollector || stationComponent.isVeinCollector)
            {
                return false;
            }

            if (__instance.player.inhandItemId > 0 && __instance.player.inhandItemCount == 0)
            {
                __instance.player.SetHandItems(0, 0);
            }
            else if (__instance.player.inhandItemId > 0 && __instance.player.inhandItemCount > 0)
            {
                int num = 5002;
                if (stationComponent.energyMax == 12000000000)
                {
                    num = 6230;
                }
                ItemProto itemProto = LDB.items.Select(num);
                if (__instance.player.inhandItemId != num)
                {
                    UIRealtimeTip.Popup("只能放入".Translate() + itemProto.name);
                    return false;
                }

                int num2 = LDB.items.Select(__instance.factory.entityPool[stationComponent.entityId].protoId)?.prefabDesc.stationMaxShipCount ?? 10;
                int num3 = stationComponent.idleShipCount + stationComponent.workShipCount;
                int num4 = num2 - num3;
                if (num4 < 0)
                {
                    num4 = 0;
                }

                int num5 = ((__instance.player.inhandItemCount < num4) ? __instance.player.inhandItemCount : num4);
                if (num5 <= 0)
                {
                    UIRealtimeTip.Popup("栏位已满".Translate());
                    return false;
                }

                int n = __instance.player.inhandItemCount;
                int m = __instance.player.inhandItemInc;
                int num6 = num5;
                int num7 = __instance.split_inc(ref n, ref m, num6);
                stationComponent.idleShipCount += num6;
                __instance.player.AddHandItemCount_Unsafe(-num6);
                __instance.player.SetHandItemInc_Unsafe(__instance.player.inhandItemInc - num7);
                if (__instance.player.inhandItemCount <= 0)
                {
                    __instance.player.SetHandItemId_Unsafe(0);
                    __instance.player.SetHandItemCount_Unsafe(0);
                    __instance.player.SetHandItemInc_Unsafe(0);
                }
            }
            else
            {
                if (__instance.player.inhandItemId != 0 || __instance.player.inhandItemCount != 0)
                {
                    return false;
                }

                int idleShipCount = stationComponent.idleShipCount;
                if (idleShipCount > 0)
                {
                    if (VFInput.shift || VFInput.control)
                    {
                        if (stationComponent.energyMax == 12000000000)
                        {
                            int upCount = __instance.player.TryAddItemToPackage(6230, idleShipCount, 0, throwTrash: false);
                            UIItemup.Up(6230, upCount);
                        }
                        else
                        {
                            int upCount = __instance.player.TryAddItemToPackage(5002, idleShipCount, 0, throwTrash: false);
                            UIItemup.Up(5002, upCount);
                        }
                    }
                    else
                    {
                        if (stationComponent.energyMax == 12000000000)
                        {
                            __instance.player.SetHandItemId_Unsafe(6230);
                        }
                        else
                        {
                            __instance.player.SetHandItemId_Unsafe(5002);
                        }
                        __instance.player.SetHandItemCount_Unsafe(idleShipCount);
                        __instance.player.SetHandItemInc_Unsafe(0);
                    }

                    stationComponent.idleShipCount = 0;
                }
            }
            return false;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationWindow), "OnStationIdChange")]
        public static void OnStationIdChangePatch(UIStationWindow __instance)
        {
            __instance.event_lock = true;
            if (__instance.active)
            {
                if (__instance.stationId == 0 || __instance.factory == null)
                {
                    return;
                }
                StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
                if (stationComponent.isStellar)
                {
                    if (stationComponent.energyMax == 12000000000)
                    {
                        
                        __instance.shipIconButton.tips.itemId = 6230;
                        ItemProto itemProto2 = LDB.items.Select(6230);
                        __instance.shipIconImage.sprite = itemProto2.iconSprite;
                    }
                    else
                    {
                        __instance.shipIconButton.tips.itemId = 5002;
                        ItemProto itemProto2 = LDB.items.Select(5002);
                        __instance.shipIconImage.sprite = itemProto2.iconSprite;
                        __instance.warperIconButton.gameObject.SetActive(false);
                        __instance.powerGroupRect.sizeDelta = new Vector2(380, 40f);
                    }
                }
            }
            __instance.event_lock = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationWindow), "_OnUpdate")]
        public static void _OnUpdatePatch(UIStationWindow __instance)
        {
            if (__instance.stationId == 0 || __instance.factory == null)
            {
                return;
            }

            StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
            bool logisticShipWarpDrive = GameMain.history.logisticShipWarpDrive;
            if (stationComponent.isStellar && logisticShipWarpDrive)
            {
                if (stationComponent.energyMax == 12000000000)
                {
                    __instance.warperIconButton.gameObject.SetActive(value: true);
                }
                else
                {
                    __instance.warperIconButton.gameObject.SetActive(value: false);
                }
                //__instance.droneIconButton.gameObject.SetActive(value: false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), "Init")]
        public static void InitPatch(StationComponent __instance, bool _logisticShipWarpDrive)
        {
            __instance.warperMaxCount = (__instance.isStellar ? (_logisticShipWarpDrive ? ((__instance.energyMax == 12000000000) ? 50 : 0) : 0) : 0);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "OnStationIdChange")]
        public static void OnStationIdChangePatch(UIControlPanelStationInspector __instance)
        {
            __instance.event_lock = true;
            if (__instance.active)
            {
                if (__instance.stationId == 0 || __instance.factory == null)
                {
                    return;
                }
                StationComponent stationComponent = __instance.transport.stationPool[__instance.stationId];
                if (stationComponent.isStellar)
                {
                    if (stationComponent.energyMax == 12000000000)
                    {
                        __instance.shipIconButton.tips.itemId = 6230;
                        ItemProto itemProto2 = LDB.items.Select(6230);
                        __instance.shipIconImage.sprite = itemProto2.iconSprite;
                    }
                    else
                    {
                        __instance.shipIconButton.tips.itemId = 5002;
                        ItemProto itemProto2 = LDB.items.Select(5002);
                        __instance.shipIconImage.sprite = itemProto2.iconSprite;
                        __instance.warperIconButton.gameObject.SetActive(false);
                        __instance.powerGroupRect.sizeDelta = new Vector2(380, 40f);
                    }
                }
            }
            __instance.event_lock = false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "UpdateDelivery")]
        public static void UpdateDeliveryPatch(UIControlPanelStationInspector __instance)
        {
            bool logisticShipWarpDrive = GameMain.history.logisticShipWarpDrive;
            if (__instance.station.isStellar && logisticShipWarpDrive)
            {
                if (__instance.station.energyMax == 12000000000)
                {
                    __instance.warperIconButton.gameObject.SetActive(value: true);
                }
                else
                {
                    __instance.warperIconButton.gameObject.SetActive(value: false);
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIControlPanelStationEntry), "_OnUpdate")]
        public static void _OnUpdatePatch(UIControlPanelStationEntry __instance)
        {
            if (__instance.factory.entityPool[__instance.station.entityId].protoId == 2104)
            {
                __instance.shipIconButton.tips.itemId = 5002;
                ItemProto itemProto2 = LDB.items.Select(5002);
                __instance.shipIconImage.sprite = itemProto2.iconSprite;
                __instance.warperIconButton.gameObject.SetActive(false);
            }
            else if (__instance.factory.entityPool[__instance.station.entityId].protoId == 6267)
            {
                __instance.shipIconButton.tips.itemId = 6230;
                ItemProto itemProto2 = LDB.items.Select(6230);
                __instance.shipIconImage.sprite = itemProto2.iconSprite;
            }
        }

    }
}
