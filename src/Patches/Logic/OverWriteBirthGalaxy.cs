using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectGenesis.Patches.Logic
{
    [HarmonyPatch]
    public static class OverWriteBirthGalaxy
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarGen), "CreateStarPlanets")]
        public static bool CreateStarPlanetsPatch(ref double[] ___pGas, GalaxyData galaxy, ref StarData star, GameDesc gameDesc)
        {
            if (!CreatePlanet(ref ___pGas, ref galaxy, ref star, ref gameDesc))
            {
                return false;
            }

            return true;
        }

        public static bool CreatePlanet(ref double[] ___pGas, ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc)
        {
            if (star.id == 1)
            {
                CreateStarPlanets(ref ___pGas, ref galaxy, ref star, ref gameDesc);
                return false;
            }
            return true;
        }

        public static void CreateStarPlanets(ref double[] ___pGas, ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc)
        {
            DotNet35Random dotNet35Random1 = new DotNet35Random(star.seed);
            dotNet35Random1.Next();
            dotNet35Random1.Next();
            dotNet35Random1.Next();
            DotNet35Random dotNet35Random2 = new DotNet35Random(dotNet35Random1.Next());
            double num1 = dotNet35Random2.NextDouble();
            double num2 = dotNet35Random2.NextDouble();
            double num3 = dotNet35Random2.NextDouble();
            double num4 = dotNet35Random2.NextDouble();
            double num5 = dotNet35Random2.NextDouble();
            double num6 = dotNet35Random2.NextDouble() * 0.2 + 0.9;
            double num7 = dotNet35Random2.NextDouble() * 0.2 + 0.9;
            DotNet35Random dotNet35Random3 = new DotNet35Random(dotNet35Random1.Next());
            AccessTools.Method(typeof(StarGen), "SetHiveOrbitsConditionsTrue").Invoke(null, new object[] { /* 传递方法所需的参数 */ });

            if (star.index == 0)
            {
                star.planetCount = 4;
                star.planets = new PlanetData[star.planetCount];

                int info_seed5 = dotNet35Random2.Next();
                int gen_seed5 = dotNet35Random2.Next();
                int info_seed6 = dotNet35Random2.Next();
                int gen_seed6 = dotNet35Random2.Next();
                int info_seed7 = dotNet35Random2.Next();
                int gen_seed7 = dotNet35Random2.Next();
                info_seed6 = dotNet35Random2.Next();
                gen_seed6 = dotNet35Random2.Next();
                star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 1, 0, false, info_seed6, gen_seed6);
                info_seed6 = dotNet35Random2.Next();
                gen_seed6 = dotNet35Random2.Next();
                star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, 2, 0, false, info_seed6, gen_seed6);
                info_seed6 = dotNet35Random2.Next();
                gen_seed6 = dotNet35Random2.Next();
                star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 2, 0, 3, 4, true, info_seed6, gen_seed6);
                info_seed6 = dotNet35Random2.Next();
                gen_seed6 = dotNet35Random2.Next();
                star.planets[3] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 3, 0, 4, 0, false, info_seed6, gen_seed6);
            }

            // 尾巴
            {
                int num16 = 0;
                int num17 = 0;
                int index1 = 0;
                for (int index2 = 0; index2 < star.planetCount; ++index2)
                {
                    if (star.planets[index2].type == EPlanetType.Gas)
                    {
                        num16 = star.planets[index2].orbitIndex;
                        break;
                    }
                }
                for (int index3 = 0; index3 < star.planetCount; ++index3)
                {
                    if (star.planets[index3].orbitAround == 0)
                        num17 = star.planets[index3].orbitIndex;
                }
                if (num16 > 0)
                {
                    int num18 = num16 - 1;
                    bool flag = true;
                    for (int index4 = 0; index4 < star.planetCount; ++index4)
                    {
                        if (star.planets[index4].orbitAround == 0 && star.planets[index4].orbitIndex == num16 - 1)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag && num4 < 0.2 + (double)num18 * 0.2)
                        index1 = num18;
                }
                int index5 = num5 >= 0.2 ? (num5 >= 0.4 ? (num5 >= 0.8 ? 0 : num17 + 1) : num17 + 2) : num17 + 3;
                if (index5 != 0 && index5 < 5)
                    index5 = 5;
                star.asterBelt1OrbitIndex = (float)index1;
                star.asterBelt2OrbitIndex = (float)index5;
                if (index1 > 0)
                    star.asterBelt1Radius = StarGen.orbitRadius[index1] * (float)num6 * star.orbitScaler;
                if (index5 > 0)
                    star.asterBelt2Radius = StarGen.orbitRadius[index5] * (float)num7 * star.orbitScaler;
                for (int index6 = 0; index6 < star.planetCount; ++index6)
                {
                    PlanetData planet = star.planets[index6];
                    AccessTools.Method(typeof(StarGen), "SetHiveOrbitConditionFalse").Invoke(null, new object[] { planet.orbitIndex, planet.orbitAroundPlanet != null ? planet.orbitAroundPlanet.orbitIndex : 0, planet.sunDistance / star.orbitScaler, star.index });

                }
                star.hiveAstroOrbits = new AstroOrbitData[8];
                AstroOrbitData[] hiveAstroOrbits = star.hiveAstroOrbits;
                int number = 0;
                for (int index7 = 0; index7 < StarGen.hiveOrbitCondition.Length; ++index7)
                {
                    if (StarGen.hiveOrbitCondition[index7])
                        ++number;
                }
                for (int index8 = 0; index8 < 8; ++index8)
                {
                    double num19 = dotNet35Random3.NextDouble() * 2.0 - 1.0;
                    double num20 = dotNet35Random3.NextDouble();
                    double num21 = dotNet35Random3.NextDouble();
                    double num22 = (double)Math.Sign(num19) * Math.Pow(Math.Abs(num19), 0.7) * 90.0;
                    double num23 = num20 * 360.0;
                    double num24 = num21 * 360.0;
                    float num25 = 0.3f;
                    Assert.Positive(number);
                    if (number > 0)
                    {
                        int num26 = star.index != 0 ? 5 : 2;
                        int maxValue = (number > num26 ? num26 : number) * 100;
                        int num27 = maxValue * 100;
                        int num28 = dotNet35Random3.Next(maxValue);
                        int num29 = num28 * num28 / num27;
                        for (int index9 = 0; index9 < StarGen.hiveOrbitCondition.Length; ++index9)
                        {
                            if (StarGen.hiveOrbitCondition[index9])
                            {
                                if (num29 == 0)
                                {
                                    num25 = StarGen.hiveOrbitRadius[index9];
                                    StarGen.hiveOrbitCondition[index9] = false;
                                    --number;
                                    break;
                                }
                                --num29;
                            }
                        }
                    }
                    hiveAstroOrbits[index8] = new AstroOrbitData();
                    hiveAstroOrbits[index8].orbitRadius = num25 * star.orbitScaler;
                    hiveAstroOrbits[index8].orbitInclination = (float)num22;
                    hiveAstroOrbits[index8].orbitLongitude = (float)num23;
                    hiveAstroOrbits[index8].orbitPhase = (float)num24;
                    hiveAstroOrbits[index8].orbitalPeriod = Math.Sqrt(39.478417604357432 * (double)num25 * (double)num25 * (double)num25 / (1.3538551990520382E-06 * (double)star.mass));
                    hiveAstroOrbits[index8].orbitRotation = Quaternion.AngleAxis(hiveAstroOrbits[index8].orbitLongitude, Vector3.up) * Quaternion.AngleAxis(hiveAstroOrbits[index8].orbitInclination, Vector3.forward);
                    hiveAstroOrbits[index8].orbitNormal = Maths.QRotateLF(hiveAstroOrbits[index8].orbitRotation, new VectorLF3(0.0f, 1f, 0.0f)).normalized;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetGen), "SetPlanetTheme")]
        public static void SetPlanetThemePatch(ref PlanetData planet, int[] themeIds, double rand1, double rand2, double rand3, double rand4, int theme_seed)
        {
            if (planet.star.index == 0)
            {
                switch (planet.index)
                {
                    case 0:
                        planet.type = EPlanetType.Vocano;
                        //planet.theme = 9;
                        break;
                    case 1:
                        planet.type = EPlanetType.Ocean;
                        //planet.theme = 1;
                        break;
                    case 2:
                        
                        break;
                    case 3:
                        planet.type = EPlanetType.Ice;
                        //planet.theme = 24;
                        break;
                }
                /*
                ThemeProto themeProto4 = LDB.themes.Select(planet.theme);
                planet.algoId = 0;
                if (themeProto4 != null && themeProto4.Algos != null && themeProto4.Algos.Length != 0)
                {
                    planet.algoId = themeProto4.Algos[(int)(rand2 * (double)themeProto4.Algos.Length) % themeProto4.Algos.Length];
                    planet.mod_x = (double)themeProto4.ModX.x + rand3 * (double)(themeProto4.ModX.y - themeProto4.ModX.x);
                    planet.mod_y = (double)themeProto4.ModY.x + rand4 * (double)(themeProto4.ModY.y - themeProto4.ModY.x);
                }

                if (themeProto4 == null)
                {
                    return false;
                }

                planet.style = theme_seed % 60;
                planet.type = themeProto4.PlanetType;
                planet.ionHeight = themeProto4.IonHeight;
                planet.windStrength = themeProto4.Wind;
                planet.waterHeight = themeProto4.WaterHeight;
                planet.waterItemId = themeProto4.WaterItemId;
                planet.levelized = themeProto4.UseHeightForBuild;
                planet.iceFlag = themeProto4.IceFlag;
                if (planet.type == EPlanetType.Gas)
                {
                    int num2 = themeProto4.GasItems.Length;
                    int num3 = themeProto4.GasSpeeds.Length;
                    int[] array = new int[num2];
                    float[] array2 = new float[num3];
                    float[] array3 = new float[num2];
                    for (int n = 0; n < num2; n++)
                    {
                        array[n] = themeProto4.GasItems[n];
                    }

                    double num4 = 0.0;
                    DotNet35Random dotNet35Random = new DotNet35Random(theme_seed);
                    for (int num5 = 0; num5 < num3; num5++)
                    {
                        float num6 = themeProto4.GasSpeeds[num5];
                        num6 *= (float)dotNet35Random.NextDouble() * 0.190909147f + 0.9090909f;
                        num6 *= 1f;
                        array2[num5] = num6 * Mathf.Pow(planet.star.resourceCoef, 0.3f);
                        ItemProto itemProto = LDB.items.Select(array[num5]);
                        array3[num5] = itemProto.HeatValue;
                        num4 += (double)(array3[num5] * array2[num5]);
                    }

                    planet.gasItems = array;
                    planet.gasSpeeds = array2;
                    planet.gasHeatValues = array3;
                    planet.gasTotalHeat = num4;
                }
                return true;
                */
            }
            //return false;
        }

        
    }
}
