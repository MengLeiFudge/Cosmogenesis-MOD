using HarmonyLib;
using ProjectOrbitalRing.Utils;
using UnityEngine;
using static ProjectOrbitalRing.Patches.Logic.ModifyUpgradeTech.ModifyUpgradeTech;
using static ProjectOrbitalRing.Patches.Logic.StarGate;
using static UIPlayerDeliveryPanel;

namespace ProjectOrbitalRing.Patches.Logic.Build
{
    internal class BuildingModelIndexRecord
    {

        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.BuildFinally))]
        [HarmonyPrefix]
        public static void BuildFinallyPatch(ref PlanetFactory __instance, int prebuildId)
        {
            if (prebuildId != 0)
            {
                PrebuildData prebuildData = __instance.prebuildPool[prebuildId];
                if (prebuildData.id == prebuildId)
                {
                    AddUniverseObserveBuilding(prebuildData.protoId);
                    RecordStarGate(prebuildData.protoId, __instance.planetId);
                }
            }
        }


        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.DismantleFinally))]
        [HarmonyPrefix]
        public static void DismantleFinallyPatch(ref PlanetFactory __instance, int objId, ref int protoId)
        {
            if (objId == 0)
            {
                return;
            }

            int num = -objId;
            if (objId > 0)
            {
                DismantleStarGate(protoId, __instance.planetId);
                DelUniverseObserveBuilding(protoId);
            }
        }
    }
}
