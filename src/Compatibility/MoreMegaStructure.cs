using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Bootstrap;
using MoreMegaStructure;
using HarmonyLib;
using ProjectGenesis.Utils;
using UnityEngine;
using xiaoye97;
using PluginInfo = BepInEx.PluginInfo;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace ProjectGenesis.Compatibility
{
    internal static class MoreMegaStructure
    {
        internal const string GUID = "Gnimaerd.DSP.plugin.MoreMegaStructure";

        private static readonly Harmony HarmonyPatch = new Harmony("ProjectGenesis.Compatibility." + GUID);

        private static readonly int[] AddedRecipes =
        {
            530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 565, 571, 572
        };

        private static bool _finished;

        internal static void Awake()
        {
            if (!Chainloader.PluginInfos.TryGetValue(GUID, out PluginInfo pluginInfo)) return;
            Assembly assembly = pluginInfo.Instance.GetType().Assembly;

            HarmonyPatch.Patch(AccessTools.Method(assembly.GetType("MoreMegaStructure.ReceiverPatchers"), "InitRawData"), null, null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(InitRawData_Transpiler)));
            HarmonyPatch.Patch(AccessTools.Method(assembly.GetType("MoreMegaStructure.UIReceiverPatchers"), "InitDict"), null, null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(InitDict_Transpiler)));

            HarmonyPatch.Patch(AccessTools.Method(assembly.GetType("MoreMegaStructure.MoreMegaStructure"), "RefreshUILabels", new[] { typeof(StarData), typeof(bool) }), null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(RefreshUILabels_Postfix)));

            HarmonyPatch.Patch(AccessTools.Method(assembly.GetType("MoreMegaStructure.MoreMegaStructure"), "UIValueUpdate"), null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(UIValueUpdate_Postfix)));

            HarmonyPatch.Patch(AccessTools.Method(assembly.GetType("MoreMegaStructure.MoreMegaStructure"), "BeforeGameTickPostPatch"), null, null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(BeforeGameTickPostPatch_Transpiler)));

            HarmonyPatch.Patch(AccessTools.Method(typeof(VFPreload), nameof(VFPreload.InvokeOnLoadWorkEnded)), null,
                new HarmonyMethod(typeof(MoreMegaStructure), nameof(LDBToolOnPostAddDataAction))
                {
                    after = new[] { LDBToolPlugin.MODGUID, },
                });
            ProjectGenesis.MoreMegaStructureCompatibility = true;
        }

        public static void LDBToolOnPostAddDataAction()
        {
            if (_finished) return;

            foreach (int recipeID in AddedRecipes)
            {
                RecipeProto recipeProto = LDB.recipes.Select(recipeID);

                if (recipeProto == null) continue;

                recipeProto.Type = (ERecipeType)10;

                switch (recipeProto.ID)
                {
                    case 534: //谐振盘
                        recipeProto.Items[2] = 7805; // 量子芯片换成量子计算机
                        break;

                    case 536: //量子服务集群
                        recipeProto.Items[0] = 7805; // 量子芯片换成量子计算机
                        break;

                    case 538: //物质解压器运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9482, 9484, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 2, 1, 2, 4 };
                        break;

                    case 539: //科学枢纽运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9481, 9486, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 3, 1, 2, 4 };
                        break;

                    case 540: //谐振发射器运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9480, 9484, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 1, 4, 2, 4 };
                        break;

                    case 541: //星际组装厂运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9487, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 2, 2, 4 };
                        break;

                    case 542: //晶体重构器运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9485, 7805, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 1, 2, 2, 4 };
                        break;

                    case 572: //恒星炮运载火箭
                        recipeProto.Type = (ERecipeType)9;
                        recipeProto.Items = new int[] { 9509, 7805, 1802, 6277 };
                        recipeProto.ItemCounts = new int[] { 2, 2, 2, 4 };
                        break;
                }
            }

            ItemProto itemProto = LDB.items.Select(9500);
            itemProto.recipes = null;
            itemProto.FindRecipes();
            itemProto.isRaw = true;

            itemProto = LDB.items.Select(9486); //量子服务集群
            itemProto.Name = "量子服务集群";
            itemProto.RefreshTranslation();

            TechProto techProto = LDB.techs.Select(1918);
            techProto.Position = new Vector2(80, 8);

            _finished = true;
        }

        public static IEnumerable<CodeInstruction> InitRawData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4, 1126));

            matcher.SetOperandAndAdvance(1127);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4, 1126));

            matcher.SetOperandAndAdvance(1127);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4, 120000000));

            matcher.SetOperandAndAdvance(600000000);

            return matcher.InstructionEnumeration();
        }

        public static IEnumerable<CodeInstruction> InitDict_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4, 1126));

            matcher.SetOperandAndAdvance(1127);

            return matcher.InstructionEnumeration();
        }


        public static void RefreshUILabels_Postfix(StarData star)
        {
            Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
            if (targetType == null) return;

            // 2. 获取字段引用
            FieldInfo set2MatDecomButtonTextTransField = AccessTools.Field(targetType, "set2MatDecomButtonTextTrans");
            if (set2MatDecomButtonTextTransField == null) return;

            //var set2MatDecomButtonTextTrans = AccessTools.Field(TargetType, "set2MatDecomButtonTextTrans");
            Transform set2MatDecomButtonTextTrans = (Transform)set2MatDecomButtonTextTransField.GetValue(null);
            set2MatDecomButtonTextTrans.GetComponent<Text>().text = "规划".Translate() + "数学率引擎".Translate();

            FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
            if (StarMegaStructureTypeField == null) return;

            int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

            FieldInfo RightDysonTitleField = AccessTools.Field(targetType, "RightDysonTitle");
            Text RightDysonTitle = (Text)RightDysonTitleField.GetValue(null);

            FieldInfo RightMaxPowGenTextField = AccessTools.Field(targetType, "RightMaxPowGenText");
            Text RightMaxPowGenText = (Text)RightMaxPowGenTextField.GetValue(null);

            int idx = star.id - 1;
            idx = idx < 0 ? 0 : (idx > 999 ? 999 : idx);
            int curtype = StarMegaStructureType[idx];
            if (curtype == 1)
            {
                RightDysonTitle.text = "数学率引擎".Translate() + " " + star.displayName;
                set2MatDecomButtonTextTrans.GetComponent<Text>().text = "当前".Translate() + " " + "数学率引擎".Translate();
                RightMaxPowGenText.text = "现实重构".Translate();
            }
        }

        public static void UIValueUpdate_Postfix()
        {
            Type targetType = AccessTools.TypeByName("MoreMegaStructure.MoreMegaStructure");
            if (targetType == null) return;

            // 2. 获取字段引用
            FieldInfo curDysonSphereField = AccessTools.Field(targetType, "curDysonSphere");
            if (curDysonSphereField == null) return;
            DysonSphere curDysonSphere = (DysonSphere)curDysonSphereField.GetValue(null);

            FieldInfo StarMegaStructureTypeField = AccessTools.Field(targetType, "StarMegaStructureType");
            if (StarMegaStructureTypeField == null) return;
            int[] StarMegaStructureType = (int[])StarMegaStructureTypeField.GetValue(null);

            FieldInfo RightMaxPowGenValueTextField = AccessTools.Field(targetType, "RightMaxPowGenValueText");
            if (RightMaxPowGenValueTextField == null) return;
            Text RightMaxPowGenValueText = (Text)RightMaxPowGenValueTextField.GetValue(null);

            if (StarMegaStructureType[curDysonSphere.starData.id - 1] == 1) //如果是数学率引擎
            {
                long DysonEnergy = (curDysonSphere.energyGenCurrentTick - curDysonSphere.energyReqCurrentTick);
                RightMaxPowGenValueText.text = (DysonEnergy / 20000) + "Humes";
            }
        }

        public static IEnumerable<CodeInstruction> BeforeGameTickPostPatch_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Stloc_0));
            Debug.LogFormat("scpppppppppppppp matcher.Opcode {0} {1}", matcher.Pos, matcher.Opcode);
            matcher.Advance(1);
            Debug.LogFormat("scpppppppppppppp1 matcher.Opcode {0} {1}", matcher.Pos, matcher.Opcode);

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldind_Ref))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StarData), nameof(StarData.id))))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1)).InsertAndAdvance(new CodeInstruction(OpCodes.Sub))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldelem_I4)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Or)).InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_0));
            Debug.LogFormat("scpppppppppppppp2 matcher.Opcode {0} {1}", matcher.Pos, matcher.Opcode);





            
            return matcher.InstructionEnumeration();
        }

    }
}
