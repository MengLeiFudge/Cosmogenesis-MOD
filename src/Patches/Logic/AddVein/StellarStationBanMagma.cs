using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;

namespace ProjectGenesis.Patches.Logic.AddVein
{
    internal class StellarStationBanMagma
    {
        [HarmonyPatch(typeof(PlanetTransport), nameof(PlanetTransport.SetStationStorage))]
        [HarmonyPrefix]
        public static void SetStationStorage(ref PlanetTransport __instance, int stationId, int storageIdx, int itemId, ref ELogisticStorage remoteLogic)
        {
            StationComponent stationComponent = __instance.GetStationComponent(stationId);
            if (stationComponent != null)
            {
                if (stationComponent.isStellar)
                {
                    if (itemId == 6202 || itemId == 6203 || itemId == 6251)
                    {
                        if (remoteLogic == ELogisticStorage.Supply)
                        {
                            remoteLogic = ELogisticStorage.None;
                        }
                    }
                }
            }
        }
    }
}
