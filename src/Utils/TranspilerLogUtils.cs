﻿using HarmonyLib;
using ProjectOrbitalRing.Compatibility;

namespace ProjectOrbitalRing.Utils
{
    public static class TranspilerLogUtils
    {
        public static void LogInstructionEnumeration(this CodeMatcher matcher)
        {
            foreach (CodeInstruction codeInstruction in matcher.InstructionEnumeration())
                ProjectOrbitalRing.logger.LogInfo(codeInstruction.ToString());
        }

        public static void LogInstructionEnumerationWhenChecking(this CodeMatcher matcher)
        {
            foreach (CodeInstruction codeInstruction in matcher.InstructionEnumeration())
                InstallationCheckPlugin.logger.LogInfo(codeInstruction.ToString());
        }
    }
}
