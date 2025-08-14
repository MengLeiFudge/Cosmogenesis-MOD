using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using static ProjectGenesis.Patches.Logic.ModifyUpgradeTech.ModifyUpgradeTech;
using static ProjectGenesis.Patches.Logic.StarGate;

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
                    UnlockedObservedTechs(prebuildData.protoId);
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
                Debug.LogFormat("==============--------------===========");
                Debug.LogFormat("protoId {0}", protoId);
                DismantleStarGate(protoId, __instance.planetId);
            }
        }
    }
}
