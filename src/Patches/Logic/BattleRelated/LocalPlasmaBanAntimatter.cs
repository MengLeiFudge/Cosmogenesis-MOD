using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGenesis.Patches.Logic.BattleRelated
{
    internal class LocalPlasmaBanAntimatter
    {
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(TurretComponent), "BeltUpdate")]
        //public static bool BeltUpdatePatch(ref TurretComponent __instance, ref CargoTraffic cargoTraffic)
        //{
        //    if (__instance.targetBeltId == 0 || __instance.itemCount >= 5)
        //    {
        //        return true;
        //    }
        //    byte stack;
        //    byte inc;
        //    int num = cargoTraffic.TryPickItem(__instance.targetBeltId, 0, 0, ItemProto.turretNeeds[(uint)__instance.ammoType], out stack, out inc);
        //    if (num == 1608)
        //    {
        //        if (__instance.type == ETurretType.LocalPlasma)
        //        {
        //            return false; // 禁止使用反物质炮弹
        //        }
        //    }
        //    return true; // 继续执行原方法
        //}
    }
}
