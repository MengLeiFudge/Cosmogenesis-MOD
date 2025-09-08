using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectGenesis.Patches.Logic.ModifyUpgradeTech.ModifyUpgradeTech;

namespace ProjectGenesis.Patches.Logic.ModifyUpgradeTech
{
    internal class UniverseObserveTech
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechaLab), "GameTick")]
        public static void GameTickPatch(MechaLab __instance)
        {
            if (!__instance.player.isAlive)
            {
                return;
            }
            int currentTech = __instance.gameHistory.currentTech;

            if (currentTech == 4101)
            {
                if (GetUniverseObserveBuilding(0) > 0)
                {
                    __instance.gameHistory.AddTechHash((long)4);
                    return;
                }
            }
            else if (currentTech == 4102)
            {
                if (GetUniverseObserveBuilding(1) > 0)
                {
                    __instance.gameHistory.AddTechHash((long)2);
                    return;
                }
            }
            else if (currentTech == 4103)
            {
                if (GetUniverseObserveBuilding(2) > 0)
                {
                    __instance.gameHistory.AddTechHash((long)1);
                    return;
                }
            }
        }
    }
}
