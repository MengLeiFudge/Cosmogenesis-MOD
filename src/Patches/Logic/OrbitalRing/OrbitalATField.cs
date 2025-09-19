using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectOrbitalRing.Patches.Logic.OrbitalRing
{
    //internal class OrbitalATField
    //{
    //    [HarmonyPatch(typeof(PlanetATField), nameof(PlanetATField.RecalculatePhysicsShape))]
    //    [HarmonyTranspiler]
    //    public static IEnumerable<CodeInstruction> RecalculatePhysicsShape_Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var matcher = new CodeMatcher(instructions);

    //        // 匹配 ldc.r4 指令
    //        matcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt), new CodeMatch(OpCodes.Callvirt));
    //        // 将操作数从 -1 修改为 6251，可建造海洋从原-1岩浆改为6251新岩浆
    //        matcher.Advance(1).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, 80f),
    //            new CodeInstruction(OpCodes.Add));

    //        return matcher.InstructionEnumeration();
    //    }

    //    [HarmonyPatch(typeof(PlanetATField), nameof(PlanetATField.UpdateGeneratorMatrix))]
    //    [HarmonyPostfix]
    //    public static void PlanetATField_UpdateGeneratorMatrix_Postfix(PlanetATField __instance)
    //    {
    //        int count = __instance.generatorCount;

    //        if (count <= 0)
    //        {
    //            return;
    //        }

    //        // 惑星内の惑星シールドジェネレータで最も高いサポート率をサポート率として採用
    //        float maxW = 0.0f;
    //        for (int i = 0; i < count; i++)
    //        {
    //            maxW = Math.Max(__instance.generatorMatrix[i].w, maxW);
    //        }

    //        __instance.generatorCount = 0;
    //        Array.Clear(__instance.generatorMatrix, 0, PlanetATField.MAX_GENERATOR_COUNT);

    //        // 惑星全体を包めるようにジェネレータを配置したことにする
    //        // 配置位置はシールドの半径をshieldRadiusと仮定して隙間がないようかつ最小限の数に抑える
    //        double shieldRadius = 80.0; // 実験した範囲だとこの値で大丈夫そう
    //        double realRadius = __instance.planet.realRadius; // 惑星の半径
    //        int rCount = (int)Math.Ceiling((Math.PI * realRadius) / (2.0 * shieldRadius));

    //        // 緯度方向に等間隔に配置する
    //        Vector4 vec;
    //        vec.x = 0.0f;
    //        vec.y = (float)realRadius - 50f;
    //        vec.z = 0.0f;
    //        vec.w = maxW;
    //        __instance.generatorMatrix[__instance.generatorCount] = vec;
    //        __instance.generatorCount++;

    //        for (int i = 1; i <= rCount; i++)
    //        {
    //            double sita = (double)i * Math.PI / (double)rCount;
    //            vec.x = 0.0f;
    //            vec.y = (float)((realRadius - 50f) * Math.Cos(sita));
    //            vec.z = (float)((realRadius - 50f) * Math.Sin(sita));
    //            vec.w = maxW;
    //            __instance.generatorMatrix[__instance.generatorCount] = vec;
    //            __instance.generatorCount++;

    //            // 緯度方向に配置したのを原点として経度方向に等間隔に配置する
    //            double r2 = (realRadius - 50f) * Math.Sin(sita);
    //            int r2Count = (int)Math.Ceiling((Math.PI * r2 * 2.0) / (2.0 * shieldRadius));
    //            for (int j = 1; j < r2Count; j++)
    //            {
    //                double sita2 = (double)j * 2.0 * Math.PI / (double)r2Count;
    //                vec.x = (float)(r2 * Math.Sin(sita2));
    //                vec.z = (float)(r2 * Math.Cos(sita2));
    //                __instance.generatorMatrix[__instance.generatorCount] = vec;
    //                __instance.generatorCount++;
    //            }
    //        }
    //    }
    //}
}
