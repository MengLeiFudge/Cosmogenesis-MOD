using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using ProjectOrbitalRing.Utils;

// ReSharper disable InconsistentNaming

namespace ProjectOrbitalRing.Compatibility
{
    internal static class WeaponPlus
    {
        internal const string GUID = "org.weaponplus.plugins.Xiaokls";

        private static readonly Harmony HarmonyPatch = new Harmony("ProjectOrbitalRing.Compatibility." + GUID);

        internal static void Awake()
        {
            if (!Chainloader.PluginInfos.TryGetValue(GUID, out PluginInfo pluginInfo)) return;

            Assembly assembly = pluginInfo.Instance.GetType().Assembly;

            Type type = assembly.GetType("DSP_WeaponPlus.Utils.WPAddItem");

            HarmonyPatch.Patch(AccessTools.Method(type, "AddDiyItem"), new HarmonyMethod(typeof(WeaponPlus), nameof(AddDiyItem_Prefix)));
        }

        public static void AddDiyItem_Prefix(int[] inputIds)
        {
            for (var i = 0; i < inputIds.Length; i++)
            {
                if (inputIds[i] == 1201) inputIds[i] = ProtoID.I熔融金属;
            }
        }
    }
}
