using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ProjectOrbitalRing.Patches.Logic.BattleRelated
{
    internal class DisturbWavePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyUnitComponent), "get_slowDownValue")]
        public static bool get_slowDownValuePatch(ref EnemyUnitComponent __instance, ref float __result)
        {
            __result = 1f - __instance.disturbValue;
            if (__result >= 1f)
            {
                __result = 1f;
            }
            else if (__result <= 0.4f)
            {
                __result = 0.4f;
            }

            return false;
        }
    }
}
