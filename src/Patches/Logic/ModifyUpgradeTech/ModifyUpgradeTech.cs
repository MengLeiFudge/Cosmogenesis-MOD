using CommonAPI.Systems;
using HarmonyLib;
using ProjectGenesis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WinAPI;
using static GalacticScale.PatchOnUIGalaxySelect;
using static ProjectGenesis.Patches.Logic.ModifyUpgradeTech.AddUpgradeTech;

namespace ProjectGenesis.Patches.Logic.ModifyUpgradeTech
{
    internal static class ModifyUpgradeTech
    {
        private static readonly int[] Items5 =
                                      {
                                          6001, 6002, 6003, 6004,
                                          6005,
                                      },
                                      Items4 =
                                      {
                                          6001, 6002, 6003, 6004,
                                      },
                                      Items3 = { 6001, 6002, 6003, },
                                      Items2 = { 6001, 6002, };

        private static readonly int[] unlockHandcraftRecipes =
        {
            21, 26, 28, 29, 34, 36, 38, 39, 41, 42, 43, 44, 47, 51, 52, 53,
            54, 57, 70, 71, 72, 73, 80, 81, 99, 100, 101, 105, 109, 115, 116, 119,
            124, 128, 132, 135, 140, 141, 142, 143, 145, 146, 153, 154, 155, 156, 157, 159,
            402, 403, 408, 416, 418, 424, 425, 519, 523, 802, 709, 710, 716, 751, 752, 754, 771,
            772, 783, 789, 793, 794, 795,
        };

        private static int vanillaTechSpeed = 1;
        private static int synapticLatheTechSpeed = 1;


        internal static void ModifyUpgradeTeches()
        {
            TechProto tech = LDB.techs.Select(ProtoID.T批量建造1);
            tech.HashNeeded = 1200;
            tech.UnlockValues = new[] { 450.0, };

            tech = LDB.techs.Select(ProtoID.T批量建造2);
            tech.UnlockValues = new[] { 900.0, };

            tech = LDB.techs.Select(ProtoID.T批量建造3);
            tech.UnlockValues = new[] { 1800.0, };

            tech = LDB.techs.Select(ProtoID.T能量回路4);
            tech.Items = Items4;
            tech.ItemPoints = Enumerable.Repeat(12, 4).ToArray();

            tech = LDB.techs.Select(ProtoID.T驱动引擎4);
            tech.Items = Items4;
            tech.ItemPoints = Enumerable.Repeat(10, 4).ToArray();

            tech = LDB.techs.Select(ProtoID.T驱动引擎5);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(10, 5).ToArray();

            tech = LDB.techs.Select(ProtoID.T垂直建造3);
            tech.Items = Items3;
            tech.ItemPoints = new[] { 20, 20, 10, };

            tech = LDB.techs.Select(ProtoID.T垂直建造6);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(6, 5).ToArray();

            tech = LDB.techs.Select(ProtoID.T集装分拣6);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(6, 5).ToArray();
            /*
            for (int i = 2501; i <= 2506; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                Debug.LogFormat("scppppppppppppperppppppppp");
                Debug.LogFormat("techProto.ID {0} techProto.Name {1} techProto.IconPath {2}", techProto.ID, techProto.Name, techProto.IconPath);
                Debug.LogFormat("techProto.Position[0] {0} techProto.Position[1] {1} ", techProto.Position[0], techProto.Position[1]);
                //Debug.LogFormat("techProto.PreTechsImplicit[0] {0} techProto.PreTechs {1}", techProto.PreTechsImplicit[0], techProto.PreTechs[0]);
                if (techProto.PreTechs != null)
                {
                    for (int j = 0; j < techProto.PreTechs.Length; j++)
                    {
                        Debug.LogFormat("techProto.PreTechs {0} j = {1}", techProto.PreTechs[j], j);
                    }
                }
                if (techProto.PreTechsImplicit != null)
                {
                    for (int j = 0; j < techProto.PreTechsImplicit.Length; j++)
                    {
                        Debug.LogFormat("techProto.PreTechsImplicit {0} j = {1}", techProto.PreTechsImplicit[j], j);
                    }
                }
                if (techProto.UnlockValues != null)
                {
                    for (int j = 0; j < techProto.UnlockValues.Length; j++)
                    {
                        Debug.LogFormat("techProto.UnlockValues {0} j = {1}", techProto.UnlockValues[j], j);
                    }
                }
                if (techProto.UnlockFunctions != null)
                {
                    for (int j = 0; j < techProto.UnlockFunctions.Length; j++)
                    {
                        Debug.LogFormat("techProto.UnlockFunctions {0} j = {1}", techProto.UnlockFunctions[j], j);
                    }
                }
            }
            */
            ModifyAllUpgradeTechs();


            ModifyCoreUpgradeTechs();
            ModifyMoveUpgradeTechs();
            ModifyPackageUpgradeTechs();
            ModifyBuilderNumberUpgradeTechs();
            ModifyReBuildUpgradeTechs();
            ModifyCombustionPowerUpgradeTechs();
            ModifyBuilderSpeedUpgradeTechs();
            ModifySolarSailingLifeUpgradeTechs();
            ModifySolarSailingAdsorbSpeedUpgradeTechs();
            ModifyRayEfficiencyUpgradeTechs();
            ModifyWhiteGrabUpgradeTechs();
            ModifySpacecraftSpeedUpgradeTechs();
            ModifySpacecraftExpansionUpgradeTechs();
            ModifyMinerUpgradeTechs();
            ModifyDamageUpgradeTechs();
            ModifyWreckageRecoveryUpgradeTechs();
            ModifyUAVHPAndfiringRateUpgradeTechs();
            ModifyGroundFormationExpansionUpgradeTechs();
            ModifySpaceFormationExpansionUpgradeTechs();
            ModifyPlanetFieldUpgradeTechs();

            AddUpgradeTechs();


            /*
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID == 3701 || techProto.ID == 3706)
                {
                    Debug.LogFormat("scppppppppppppperppppppppp");
                    Debug.LogFormat("techProto.ID {0} techProto.Name {1} techProto.IconPath {2}", techProto.ID, techProto.Name, techProto.IconPath);
                    if (techProto.UnlockValues != null)
                    {
                        for (int j = 0; j < techProto.UnlockValues.Length; j++)
                        {
                            Debug.LogFormat("techProto.UnlockValues {0} j = {1}", techProto.UnlockValues[j], j);
                        }
                    }
                }
            }
            */

            tech = LDB.techs.Select(4102);
            tech.Items = Items2;
            tech.ItemPoints = Enumerable.Repeat(10, 2).ToArray();

            tech = LDB.techs.Select(4103);
            tech.Items = new[] { 6003, };
            tech.ItemPoints = new[] { tech.ItemPoints[0], };

            tech = LDB.techs.Select(ProtoID.T宇宙探索4);
            tech.Items = new[] { 6003, 6278 };
            tech.ItemPoints = new[] { tech.ItemPoints[0], tech.ItemPoints[1], };

            

            for (int i = 3701; i <= 3706; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new[] { 1d, 1d };
            }

            /*
            for (int i = ProtoID.T宇宙探索1; i <= ProtoID.T宇宙探索4; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                techProto.Items = new[] { 6001, };
                techProto.ItemPoints = new[] { techProto.ItemPoints[0], };
                techProto.PreTechsImplicit = Array.Empty<int>();
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID < 2000) continue;

                Debug.LogFormat("techProto.Desc {0} techProto.Items {1} techProto.ItemPoints {2}", techProto.Desc, techProto.Items, techProto.ItemPoints);
                Debug.LogFormat("techProto.HashNeeded {0} techProto.UnlockFunctions {1} techProto.UnlockValues {2}", techProto.HashNeeded, techProto.UnlockFunctions, techProto.UnlockValues);
                Debug.LogFormat("techProto.Level {0} techProto.MaxLevel {1} techProto.LevelCoef1 {2} techProto.LevelCoef2 {3}", techProto.Level, techProto.MaxLevel, techProto.LevelCoef1, techProto.LevelCoef2);
                Debug.LogFormat("techProto.PreTechsMax {0}", techProto.PreTechsMax);

                int[] items = techProto.Items;

                if (items.SequenceEqual(Items5))
                {
                    techProto.Items = new[] { 6280, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[4], };
                    continue;
                }

                if (items.SequenceEqual(Items4))
                {
                    if (techProto.ID % 100 > 2)
                    {
                        techProto.Items = new[] { 6278, 6279, };
                        techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[3], };
                    }
                    else
                    {
                        techProto.Items = new[] { 6278, 6003, 6004, };
                        techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[2], techProto.ItemPoints[3], };
                    }

                    continue;
                }

                if (items.SequenceEqual(Items3))
                {
                    techProto.Items = new[] { 6278, 6003, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[2], };
                    continue;
                }

                // ReSharper disable once InvertIf
                if (items.SequenceEqual(Items2) && techProto.ID % 100 > 2)
                {
                    techProto.Items = new[] { 6278, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0], };
                    continue;
                }
            }*/
        }

        internal static void ModifyAllUpgradeTechs()
        {
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID < 2000) continue;

                int[] items = techProto.Items;

                if (items.SequenceEqual(Items5))
                {
                    techProto.Items = new[] { 6279, 6004 };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0], techProto.ItemPoints[0]};
                    continue;
                }

                if (items.SequenceEqual(Items4))
                {
                    techProto.Items = new int[] { 6003, 6278 };
                    techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                    continue;
                }

                if (items.SequenceEqual(Items3))
                {
                    techProto.Items = new[] { 6003, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0] };
                    continue;
                }
            }
        }

        internal static void ModifyCoreUpgradeTechs()
        {
            TechProto techProto;
            double coreNenergy = 0;
            for (int i = 2101; i <= 2105; i++)
            {
                techProto = LDB.techs.Select(i);
                coreNenergy = techProto.UnlockValues[0];
                techProto.UnlockFunctions = new[] { 6, 82, 83, };
                techProto.UnlockValues = new double[] { coreNenergy, 4d, 1000d, };

                switch (i)
                {
                    case 2101:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2102:
                        break;
                    case 2103:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2104:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2105:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
            TechProto coreInfinItetechProto = LDB.techs.Select(2106);
            coreNenergy = coreInfinItetechProto.UnlockValues[0];
            coreInfinItetechProto.UnlockFunctions = new[] { 6, 83, };
            coreInfinItetechProto.UnlockValues = new double[] { coreNenergy, 200d, };
        }

        internal static void ModifyMoveUpgradeTechs()
        {
            TechProto techProto;
            double moveSpped = 0;
            for (int i = 2201; i <= 2208; i++)
            {
                techProto = LDB.techs.Select(i);
                moveSpped = techProto.UnlockValues[0];
                techProto.UnlockFunctions = new int[] { 3, 81 };
                techProto.UnlockValues = new double[] { moveSpped, moveSpped * 75000 };
                switch (i)
                {
                    case 2201:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2202:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2203:
                        break;
                    case 2204:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2205:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2206:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2207:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2208:
                        techProto.Items = new int[] { 6279, 6004, 6005 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyPackageUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2301; i <= 2307; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 2301:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2302:
                        break;
                    case 2303:
                        techProto.Items = new int[] { 6001, 6002 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2304:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2305:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2306:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2307:
                        techProto.Items = new int[] { 6279, 6004, 6005 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyBuilderNumberUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2404; i <= 2406; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 2404:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2305:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2306:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyReBuildUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2953; i <= 2956; i++)
            {
                techProto = LDB.techs.Select(i);

                switch (i)
                {
                    case 2953:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2954:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2955:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2956:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyCombustionPowerUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2501; i <= 2506; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { techProto.UnlockValues[0] * 2 };

                switch (i)
                {
                    case 2501:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2502:
                    case 2503:
                        break;
                    case 2504:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2506:
                        break;
                    default:
                        break;
                }
            }
        }

        
        internal static void ModifyBuilderSpeedUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(2606);
            techProto.MaxLevel = 21;
        }

        internal static void ModifySolarSailingLifeUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3106);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySolarSailingAdsorbSpeedUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 4201; i <= 4206; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, 6005 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
        }

        internal static void ModifyRayEfficiencyUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3207);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifyWhiteGrabUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 3313; i <= 3314; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6003, 6278 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
            techProto = LDB.techs.Select(3315);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3316);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpacecraftSpeedUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3406);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpacecraftExpansionUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3508);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifyMinerUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3604);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3605);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3606);
            techProto.MaxLevel = 11;
        }

        internal static void ModifyDamageUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5004; i <= 5005; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5006);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T动能武器伤害无限";
            techProto.RefreshTranslation();

            for (int i = 5104; i <= 5105; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5106);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T能量武器伤害无限";
            techProto.RefreshTranslation();

            for (int i = 5204; i <= 5205; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5206);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T爆破武器伤害无限";
            techProto.RefreshTranslation();
        }

        internal static void ModifyWreckageRecoveryUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5301; i <= 5305; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Name = "残骸回收分析";
                techProto.Desc = "T残骸回收分析";
                techProto.RefreshTranslation();
                techProto.IconPath = "Assets/texpack/回收科技";

                switch (i)
                {
                    case 5301:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 3 };
                        break;
                    case 5302:
                        techProto.Items = new int[] { 6001, 6002 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 6 };
                        break;
                    case 5303:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 9 };
                        break;
                    case 5304:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 12 };
                        break;
                    case 5305:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 15 };
                        break;
                    default:
                        break;
                }
            }
            techProto = LDB.techs.Select(5301);
            techProto.PreTechsImplicit = new[] { 1826, };

            techProto = LDB.techs.Select(5305);
            techProto.MaxLevel = 5;

        }

        internal static void ModifyUAVHPAndfiringRateUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5601; i <= 5605; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 5601:
                        techProto.Name = "机兵升级计划";
                        techProto.Desc = "T机兵升级计划";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69};
                        techProto.UnlockValues = new double[] { 0.1, 0.05};
                        break;
                    case 5602:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.1, 0.1 };
                        break;
                    case 5603:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.2, 0.1 };
                        break;
                    case 5604:
                        techProto.Name = "军械量产方案";
                        techProto.Desc = "T军械量产方案";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.3, 0.05 };
                        break;
                    case 5605:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.3, 0.2 };
                        break;
                }

            }
            techProto = LDB.techs.Select(5601);
            techProto.PreTechsImplicit = new[] { 1819, };
        }

        internal static void ModifyGroundFormationExpansionUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(5801);
            techProto.UnlockFunctions = new int[] { 77 };
            techProto.UnlockValues = new double[] { 1 };

            techProto = LDB.techs.Select(5803);
            techProto.UnlockFunctions = new int[] { 78 };
            techProto.UnlockValues = new double[] { 2 };

            techProto = LDB.techs.Select(5806);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpaceFormationExpansionUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5901; i <= 5903; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
            for (int i = 5904; i <= 5905; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, 6005 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
        }

        internal static void ModifyPlanetFieldUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(5702);
            techProto.Items = new int[] { 6003, 6278 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(5703);
            techProto.Items = new int[] { 6279, 6004, };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(5704);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.NotifyTechUnlock))]
        [HarmonyPrefix]
        public static void NotifyTechUnlockPatch(GameHistoryData __instance, int _techId)
        {
            //Debug.LogFormat("scpppppppeopppppppppp UnlockTech techId {0}", _techId);
            CombatDroneMotify(_techId);
            WreckFalling(_techId);
            CrackingRayTechAndItemModify(_techId);
            UnlockRecipesHandcraft(_techId);
            UpdateTechSpeed(_techId);
        }


        static void CombatDroneMotify(int techId)
        {
            ItemProto itemProto;
            RecipeProto recipeProto;
            if (techId == 5601)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[2] = 1303;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[3] = 1113;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[3] = 1113;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机A型";
                itemProto.Description = "I攻击无人机A型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机A型";
                itemProto.Description = "I精准无人机A型";
                itemProto.RefreshTranslation();

            }
            else if (techId == 5602)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[0] = 1107;
                recipeProto.Items[3] = 1126;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机B型";
                itemProto.Description = "I攻击无人机B型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机B型";
                itemProto.Description = "I精准无人机B型";
                itemProto.RefreshTranslation();
            }
            else if (techId == 5603)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[0] = 6225;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[2] = 1206;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[2] = 1206;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机C型";
                itemProto.Description = "I攻击无人机C型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机C型";
                itemProto.Description = "I精准无人机C型";
                itemProto.RefreshTranslation();
            }
            else if (techId == 5604)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Type = ERecipeType.Assemble;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Type = ERecipeType.Assemble;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Type = ERecipeType.Assemble;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机D型";
                itemProto.Description = "I攻击无人机D型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机D型";
                itemProto.Description = "I精准无人机D型";
                itemProto.RefreshTranslation();
            }
            else if (techId == 5605)
            {
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[3] = 1118;
                recipeProto.Items[2] = 6243;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[3] = 1118;
                recipeProto.Items[2] = 6243;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机E型";
                itemProto.Description = "I攻击无人机E型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机E型";
                itemProto.Description = "I精准无人机E型";
                itemProto.RefreshTranslation();
            }
        }

        static void WreckFalling(int techId)
        {
            ItemProto itemProto;
            if (techId == 5301)
            {
                // 解锁3级的黑雾掉落
                itemProto = LDB.items.Select(1108);
                itemProto.EnemyDropCount = 1.8f;
                itemProto = LDB.items.Select(1109);
                itemProto.EnemyDropCount = 1.6f;
                itemProto = LDB.items.Select(1112);
                itemProto.EnemyDropCount = 1.0f;
                itemProto = LDB.items.Select(1202);
                itemProto.EnemyDropCount = 2.0f;
                itemProto = LDB.items.Select(1301);
                itemProto.EnemyDropCount = 2.0f;
                itemProto = LDB.items.Select(5206);
                itemProto.EnemyDropCount = 2.5f;
                ItemProto.InitEnemyDropTables();
            }
            else if (techId == 5302)
            {
                // 解锁6级的黑雾掉落
                itemProto = LDB.items.Select(1103);
                itemProto.EnemyDropCount = 1.56f;
                itemProto = LDB.items.Select(1105);
                itemProto.EnemyDropCount = 1.8f;
                itemProto = LDB.items.Select(1111);
                itemProto.EnemyDropCount = 1.4f;
                itemProto = LDB.items.Select(1131);
                itemProto.EnemyDropCount = 2.0f;
                itemProto = LDB.items.Select(1203);
                itemProto.EnemyDropCount = 1.2f;
                itemProto = LDB.items.Select(1401);
                itemProto.EnemyDropCount = 1.3f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5303)
            {
                // 解锁9级的黑雾掉落
                itemProto = LDB.items.Select(1106);
                itemProto.EnemyDropCount = 2.2f;
                itemProto = LDB.items.Select(1115);
                itemProto.EnemyDropCount = 1.0f;
                itemProto = LDB.items.Select(1123);
                itemProto.EnemyDropCount = 1.0f;
                itemProto = LDB.items.Select(1204);
                itemProto.EnemyDropCount = 0.8f;
                itemProto = LDB.items.Select(1404);
                itemProto.EnemyDropCount = 1.4f;
                itemProto = LDB.items.Select(1407);
                itemProto.EnemyDropCount = 1.0f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5304)
            {
                // 解锁12级的黑雾掉落
                itemProto = LDB.items.Select(1107);
                itemProto.EnemyDropCount = 1.1f;
                itemProto = LDB.items.Select(1113);
                itemProto.EnemyDropCount = 1.2f;
                itemProto = LDB.items.Select(1119);
                itemProto.EnemyDropCount = 1.4f;
                itemProto = LDB.items.Select(1124);
                itemProto.EnemyDropCount = 0.8f;
                itemProto = LDB.items.Select(1205);
                itemProto.EnemyDropCount = 0.5f;
                itemProto = LDB.items.Select(1405);
                itemProto.EnemyDropCount = 0.6f;
                itemProto = LDB.items.Select(5201);
                itemProto.EnemyDropCount = 2.5f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5305)
            {
                // 解锁15级的黑雾掉落
                itemProto = LDB.items.Select(1118);
                itemProto.EnemyDropCount = 0.8f;
                itemProto = LDB.items.Select(1118);
                itemProto.EnemyDropCount = 0.8f;
                itemProto = LDB.items.Select(1126);
                itemProto.EnemyDropCount = 0.55f;
                itemProto = LDB.items.Select(1303);
                itemProto.EnemyDropCount = 0.75f;
                itemProto = LDB.items.Select(5203);
                itemProto.EnemyDropCount = 1.5f;
                itemProto = LDB.items.Select(6222);
                itemProto.EnemyDropCount = 1.2f;
                itemProto = LDB.items.Select(7804);
                itemProto.EnemyDropCount = 0.7f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5306) {
                // 解锁18级的黑雾掉落
                itemProto = LDB.items.Select(1125);
                itemProto.EnemyDropCount = 0.4f;
                itemProto = LDB.items.Select(1127);
                itemProto.EnemyDropCount = 0.4f;
                itemProto = LDB.items.Select(1206);
                itemProto.EnemyDropCount = 0.4f;
                itemProto = LDB.items.Select(1402);
                itemProto.EnemyDropCount = 0.25f;
                itemProto = LDB.items.Select(5202);
                itemProto.EnemyDropCount = 1.5f;
                itemProto = LDB.items.Select(6201);
                itemProto.EnemyDropCount = 0.6f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5307)
            {
                // 解锁21级的黑雾掉落
                itemProto = LDB.items.Select(1014);
                itemProto.EnemyDropCount = 2f;
                itemProto = LDB.items.Select(1210);
                itemProto.EnemyDropCount = 0.3f;
                itemProto = LDB.items.Select(1304);
                itemProto.EnemyDropCount = 0.4f;
                itemProto = LDB.items.Select(1406);
                itemProto.EnemyDropCount = 0.3f;
                itemProto = LDB.items.Select(5204);
                itemProto.EnemyDropCount = 1.4f;
                itemProto = LDB.items.Select(6243);
                itemProto.EnemyDropCount = 0.4f;
                ItemProto.InitEnemyDropTables();
            } else if (techId == 5308)
            {
                // 解锁24级的黑雾掉落
                itemProto = LDB.items.Select(1016);
                itemProto.EnemyDropCount = 2.0f;
                itemProto = LDB.items.Select(1209);
                itemProto.EnemyDropCount = 0.4f;
                itemProto = LDB.items.Select(5205);
                itemProto.EnemyDropCount = 1.5f;
                itemProto = LDB.items.Select(6271);
                itemProto.EnemyDropCount = 0.3f;
                itemProto = LDB.items.Select(7805);
                itemProto.EnemyDropCount = 0.3f;
                ItemProto.InitEnemyDropTables();
            }
        }

        static void CrackingRayTechAndItemModify(int techId)
        {
            ItemProto itemProto;
            TechProto techProto;
            if (techId == 1945)
            {
                itemProto = LDB.items.Select(6216);
                itemProto.Name = "裂解射线发生器";
                itemProto.Description = "I裂解射线发生器";
                itemProto.RefreshTranslation();

                techProto = LDB.techs.Select(1945);
                techProto.Name = "终末螺旋";
                techProto.Desc = "T终末螺旋";
                techProto.RefreshTranslation();
            }
        }

        static void UnlockRecipesHandcraft(int techId)
        {
            RecipeProto recipeProto;
            if (techId == 1945)
            {
                for (int i = 0; i < unlockHandcraftRecipes.Length; i++)
                {
                    recipeProto = LDB.recipes.Select(unlockHandcraftRecipes[i]);
                    recipeProto.Handcraft = true;
                }
            }
        }

        static void UpdateTechSpeed(int techId)
        {
            TechProto techProto = LDB.techs.Select(techId);

            if (techProto.UnlockFunctions.Length > 0 && techProto.UnlockFunctions[0] == 22)
            {
                vanillaTechSpeed++;
            }
        }


        [HarmonyPatch(typeof(TechProto), nameof(TechProto.UnlockFunctionText))]
        [HarmonyPrefix]
        public static bool UnlockFunctionTextPatch(TechProto __instance, StringBuilder sb, ref string __result)
        {
            string text = "";
            if (__instance.UnlockFunctions.Length > 0)
            {
                if (__instance.UnlockFunctions[0] == 101)
                {
                    text = text + "黑雾".Translate() + __instance.UnlockValues[0] + "级残骸物品掉落".Translate();
                    __result = text;
                    return false;
                }
                if (__instance.UnlockFunctions.Length > 1) {
                    if (__instance.UnlockFunctions[0] == 7 && __instance.UnlockFunctions[1] == 102)
                    {
                        text = text + "+" + __instance.UnlockValues[0].ToString("0%") + "手动合成速度".Translate();
                        text += "\r\n";
                        text = text + "解锁手搓".Translate();
                        __result = text;
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.TryAddItemToPackage))]
        [HarmonyPrefix]
        public static bool TryAddItemToPackagePatch(ref Player __instance, int itemId, int count, ref int __result)
        {
            if (itemId == 6254 && count > 0)
            {
                RecipeProto recipeProto;
                
                if (__instance.mecha.gameData.history.currentTech > 0)
                {
                    if (LDB.techs.Select(__instance.mecha.gameData.history.currentTech).IsLabTech == false)
                    {
                        recipeProto = LDB.recipes.Select(533);
                        if (recipeProto.ItemCounts[0] == 2)
                        {
                            vanillaTechSpeed = __instance.mecha.gameData.history.techSpeed;
                        }
                        recipeProto.ItemCounts[0] = recipeProto.ItemCounts[0] * 2;
                        __instance.mecha.gameData.history.techSpeed += synapticLatheTechSpeed * 2;
                        __result = 0;
                        return false;
                    }
                    else
                    {
                        recipeProto = LDB.recipes.Select(533);
                        recipeProto.ItemCounts[0] = 2;
                        __instance.mecha.gameData.history.techSpeed = vanillaTechSpeed;
                        __result = 0;
                        return false;
                    }
                } else
                {
                    recipeProto = LDB.recipes.Select(533);
                    if (recipeProto.ItemCounts[0] == 2)
                    {
                        vanillaTechSpeed = __instance.mecha.gameData.history.techSpeed;
                    }
                    recipeProto.ItemCounts[0] = recipeProto.ItemCounts[0] * 2;
                    __instance.mecha.gameData.history.techSpeed += synapticLatheTechSpeed * 2;
                    __result = 0;
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.RemoveTechInQueue))]
        [HarmonyPrefix]
        public static void RemoveTechInQueuePatch(GameHistoryData __instance, int index)
        {
            if (index == 0) { 
                RecipeProto recipeProto;
                recipeProto = LDB.recipes.Select(533);
                recipeProto.ItemCounts[0] = 2;
                __instance.techSpeed = vanillaTechSpeed;
            }
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.DequeueTech))]
        [HarmonyPrefix]
        public static void DequeueTechPatch(GameHistoryData __instance)
        {
            RecipeProto recipeProto;
            recipeProto = LDB.recipes.Select(533);
            recipeProto.ItemCounts[0] = 2;
            __instance.techSpeed = vanillaTechSpeed;
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.EnqueueTech))]
        [HarmonyPrefix]
        public static void EnqueueTechPatch(GameHistoryData __instance, int techId)
        {
            if (__instance.techQueue[0] == 0)
            {
                if (LDB.techs.Select(techId).IsLabTech == true)
                {
                    RecipeProto recipeProto;
                    recipeProto = LDB.recipes.Select(533);
                    recipeProto.ItemCounts[0] = 2;
                    __instance.techSpeed = vanillaTechSpeed;
                }
            }
        }

        internal static void Export(BinaryWriter w)
        {

            w.Write(vanillaTechSpeed);
            w.Write(synapticLatheTechSpeed);
        }

        internal static void Import(BinaryReader r)
        {
            try
            {
                vanillaTechSpeed = r.ReadInt32();
                synapticLatheTechSpeed = r.ReadInt32();
            }
            catch (EndOfStreamException)
            {
                // ignored
            }
        }
    }
}
