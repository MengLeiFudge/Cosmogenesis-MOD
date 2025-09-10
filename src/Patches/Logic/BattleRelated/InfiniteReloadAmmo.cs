using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOrbitalRing.Patches.Logic.BattleRelated
{
    internal class InfiniteReloadAmmo
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "LoadAmmo")]
        public static void LoadAmmoPatch(ref TurretComponent __instance)
        {
            if (__instance.itemId == 7609)
            {
                if (__instance.itemCount >= 1)
                {
                    __instance.itemCount++;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "LoadAmmo")]
        public static void LoadAmmoPatch(Mecha __instance)
        {
            StorageComponent.GRID[] grids = __instance.ammoStorage.grids;
            int num = __instance.ammoSelectSlot - 1;
            int size = __instance.ammoStorage.size;
            if (grids[num].count == 0)
            {
                for (int i = num; i < num + size; i++)
                {
                    int num3 = i % size;
                    if (grids[num3].itemId == __instance.ammoItemId && grids[num3].count > 0 && grids[num3].itemId == 7609)
                    {
                        grids[num3].count++;
                    }

                }
            }
            if (grids[num].itemId == 7609)
            {
                grids[num].count++;
            }
        }

    }
}
