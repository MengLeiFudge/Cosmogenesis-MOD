using HarmonyLib;
using ProjectOrbitalRing.Patches.UI.Utils;
using ProjectOrbitalRing.Utils;

// ReSharper disable InconsistentNaming

namespace ProjectOrbitalRing.Patches.UI.PlanetFocus
{
    public static class UIPlanetDetailExpand
    {
        private static UIButton _planetFocusBtn;

        [HarmonyPatch(typeof(UIGame), nameof(UIGame._OnInit))]
        [HarmonyPostfix]
        public static void Init(UIGame __instance)
        {
            if (_planetFocusBtn) return;

            ProjectOrbitalRing.PlanetFocusWindow = UIPlanetFocusWindow.CreateWindow();

            _planetFocusBtn = Util.CreateButton("星球基地".TranslateFromJson());
            Util.NormalizeRectWithTopLeft(_planetFocusBtn, 5, -40, __instance.planetDetail.rectTrans);
            _planetFocusBtn.onClick += _ => ProjectOrbitalRing.PlanetFocusWindow.OpenWindow();
        }

        [HarmonyPatch(typeof(UIPlanetDetail), nameof(UIPlanetDetail.OnPlanetDataSet))]
        [HarmonyPostfix]
        public static void OnPlanetDataSet_Postfix(UIPlanetDetail __instance)
        {
            if (__instance.planet == null)
            {
                ProjectOrbitalRing.PlanetFocusWindow._Close();

                return;
            }

            bool notgas = __instance.planet.type != EPlanetType.Gas && GameMain.history.TechUnlocked(ProtoID.T行星协调中心);

            if (_planetFocusBtn) _planetFocusBtn.gameObject.SetActive(notgas);

            if (notgas)
            {
                ProjectOrbitalRing.PlanetFocusWindow.nameText.text = __instance.planet.displayName + " - " + "星球倾向".TranslateFromJson();

                if (UIPlanetFocusWindow.CurPlanetId != __instance.planet.id)
                {
                    UIPlanetFocusWindow.CurPlanetId = __instance.planet.id;
                    ProjectOrbitalRing.PlanetFocusWindow.OnPlanetChanged(UIPlanetFocusWindow.CurPlanetId);
                }
            }
        }
    }
}
