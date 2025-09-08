using HarmonyLib;
using ProjectGenesis.Utils;
using UnityEngine;
using static ProjectGenesis.Patches.Logic.ModifyUpgradeTech.ModifyUpgradeTech;
using static ProjectGenesis.Patches.Logic.StarGate;
using static UIPlayerDeliveryPanel;

namespace ProjectGenesis.Patches.Logic.Build
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
                    Debug.LogFormat("==============--------------===========");
                    Debug.LogFormat("prebuildData.pos.x {0}, prebuildData.pos.y {1}, prebuildData.pos.z {2}", prebuildData.pos.x, prebuildData.pos.y, prebuildData.pos.z);
                    Debug.LogFormat("prebuildData.pos2.x {0}, prebuildData.pos2.y {1}, prebuildData.pos2.z {2}", prebuildData.pos2.x, prebuildData.pos2.y, prebuildData.pos2.z);
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
