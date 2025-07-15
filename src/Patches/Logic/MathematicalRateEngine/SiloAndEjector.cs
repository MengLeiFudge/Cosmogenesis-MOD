using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGenesis.Patches.Logic.MathematicalRateEngine
{
    internal class SiloAndEjector
    {
        /// <summary>
        /// 火箭发射器所需火箭修正，注意如果更改了巨构类型，而发射器内还存有不相符的火箭，该火箭将直接消失（为了防止用廉价火箭白嫖高价火箭）
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static void SiloUpdatePatch(ref SiloComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2312) return; //只修改原始火箭发射器

            if (starIndex < 0 || starIndex > 999)
            {
                //Debug.LogWarning("SiloInternalUpdate Patch Error because starIndex out of range.");
                return;
            }

            int bulletIdExpected = 1503;

            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole)
            {
                if (!GameMain.history.TechUnlocked(1952))
                {
                    bulletIdExpected = 6228;
                }
                else if (!GameMain.history.TechUnlocked(1960))
                {
                    bulletIdExpected = 6504;
                }
                else
                {
                    bulletIdExpected = 6502;
                }

                //if (__instance.bulletId != bulletIdExpected)
                //{
                    //if (ProjectGenesis.MoreMegaStructureCompatibility)
                    //{
                    //    Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
                    //    if (targetType == null) return;

                    //    FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
                    //    if (StarMegaStructureTypeField == null) return;
                    //    int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

                    //    int megaType = StarMegaStructureType[starIndex];
                    //    switch (megaType)
                    //    {
                    //        case 1:
                    //            bulletIdExpected = 9488;
                    //            break;

                    //        case 2:
                    //            bulletIdExpected = 9489;
                    //            break;

                    //        case 3:
                    //            bulletIdExpected = 9490;
                    //            break;

                    //        case 4:
                    //            bulletIdExpected = 9491;
                    //            break;

                    //        case 5:
                    //            bulletIdExpected = 9492;
                    //            break;

                    //        case 6:
                    //            bulletIdExpected = 9510;
                    //            break;
                    //    }
                    //}
                //}
            }
            bool knownId = false; // 此处是为了适配深空来敌mod，有其他的火箭需要借用游戏本体的silo发射，因此只有已知的id会进行转化，位置的id交由深空来敌mod进行处理
            switch (__instance.bulletId)
            {
                case 1503:
                case 9488:
                case 9489:
                case 9490:
                case 9491:
                case 9492:
                case 9510:
                case 6228:
                case 6504:
                case 6502:
                    knownId = true;
                    break;
            }
            if (__instance.bulletId != bulletIdExpected && knownId)
            {
                __instance.bulletCount = 0;
                __instance.bulletInc = 0;
                __instance.bulletId = bulletIdExpected;
            }
            
        }

        /// <summary>
        /// 弹射器所需发射物修正，类似上面的发射井
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static void EjectorUpdatePatch(ref EjectorComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2311) return; //只修改原始弹射器

            if (starIndex < 0 || starIndex > 999)
            {
                return;
            }
            int bulletIdExpected = 1501;
            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole)
            {
                if (!GameMain.history.TechUnlocked(1802))
                {
                    bulletIdExpected = 6228;
                }
                else if (!GameMain.history.TechUnlocked(1952))
                {
                    bulletIdExpected = 9480;
                }
                else if (!GameMain.history.TechUnlocked(1960))
                {
                    bulletIdExpected = 1803;
                }
                else
                {
                    bulletIdExpected = 9482;
                }
                //if (__instance.bulletId != bulletIdExpected)
                //{
                    //if (ProjectGenesis.MoreMegaStructureCompatibility)
                    //{
                    //    Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
                    //    if (targetType == null) return;

                    //    FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
                    //    if (StarMegaStructureTypeField == null) return;
                    //    int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

                    //    if (StarMegaStructureType[starIndex] != 0)
                    //    {
                    //        if (StarMegaStructureType[starIndex] == 2)
                    //        {
                    //            bulletIdExpected = 6006;
                    //        }
                    //        else
                    //        {
                    //            bulletIdExpected = 1501;
                    //        }
                    //    }
                    //}
                }
                if (__instance.bulletId != bulletIdExpected)
                {
                    __instance.bulletCount = 0;
                    __instance.bulletInc = 0;
                    __instance.bulletId = bulletIdExpected;
                }
            //}
            
        }
    }
}
