using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace ProjectGenesis.Patches.Logic
{
    public static class GeothermalStrengthPatches
    {
        [HarmonyPatch(typeof(PowerSystem), nameof(PowerSystem.CalculateGeothermalStrength))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SetTargetCargoBytes_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            // 匹配 ldc.i4.m1 指令
            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_M1));

            // 将操作数从 -1 修改为 6251，可建造海洋从原-1岩浆改为6251新岩浆
            matcher.SetAndAdvance(OpCodes.Ldc_I4, 6251);

            //matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 3f));

            //matcher.SetOperandAndAdvance(2f);

            return matcher.InstructionEnumeration();
        }
    }
}
