using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CommonAPI;
using GalacticScale;
using HarmonyLib;
using ProjectGenesis.Utils;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.UI.GridLayoutGroup;
using static UnityEngine.UI.Image;
using static UnityEngine.UI.InputField;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable Unity.UnknownResource

namespace ProjectGenesis.Patches.Logic.AddVein
{
    public static partial class AddVeinPatches
    {
        internal const sbyte VeinTypeCount = 20;

        private static readonly Color32[] VeinColors =
        {
            new Color(0.538f, 0.538f, 0.538f), // Default
            new Color(0.288f, 0.587f, 0.858f), // 铁
            new Color(1.000f, 0.490f, 0.307f), // 铜
            new Color(0.214f, 0.745f, 0.531f), // 硅
            new Color(1.000f, 1.000f, 1.000f), // 钛
            new Color(0.483f, 0.461f, 0.444f), // 石
            new Color(0.113f, 0.130f, 0.140f), // 煤
            new Color(0.000f, 0.000f, 0.000f), // 油
            new Color(1.000f, 1.000f, 1.000f), // 可燃冰
            new Color(0.489f, 0.601f, 0.745f), // 金伯利
            new Color(0.066f, 0.290f, 0.160f), // 莫桑石
            new Color(0.538f, 0.613f, 0.078f), // 有机
            new Color(0.575f, 0.270f, 0.830f), // 光栅
            new Color(0.571f, 0.708f, 0.647f), // 刺笋
            new Color(0.349f, 0.222f, 0.247f), // 单极
            new Color(0.113f, 0.130f, 0.140f), // 石墨
            new Color(0.538f, 0.538f, 0.538f), // 深层岩浆
            new Color(0.685f, 0.792f, 0.000f), // 放射
            new Color(0.965f, 0.867f, 0.352f), // 黄铁
            new Color(0.000f, 0.000f, 0.000f), // 冰
        };

        internal static void ModifyVeinData()
        {
            AddVeinProtos(
                NewVein(15, "石墨矿脉", "I石墨矿", "Assets/texpack/钨矿脉", ProtoID.I石墨矿, 34, 1, 60),
                NewVein(16, "深层岩浆", "I深层岩浆", "Icons/Vein/oil-vein", ProtoID.I深层岩浆, 0, 6, 60),
                NewVein(17, "铀矿脉", "I铀矿", "Assets/texpack/放射晶体矿脉_新新", ProtoID.I放射性矿物, 35, 2, 90),
                NewVein(18, "黄铁矿脉", "I黄铁矿", "Assets/texpack/硫矿脉_新", ProtoID.I黄铁矿, 36, 1, 90),
                NewVein(19, "地下冰层", "I地下冰层", "Icons/Vein/oil-vein", ProtoID.I水, 0, 6, 60));
            return;

            VeinProto NewVein(int id, string name, string description, string iconPath, int miningItem, int miningEffect, int modelIndex,
                int miningTime) =>
                new VeinProto
                {
                    ID = id,
                    Name = name,
                    Description = description,
                    IconPath = iconPath,
                    MiningItem = miningItem,
                    MiningEffect = miningEffect,
                    ModelIndex = modelIndex,
                    MiningTime = miningTime,
                    CircleRadius = 1,
                    MinerBaseModelIndex = 58,
                    MinerCircleModelIndex = 59,
                    MiningAudio = 122,
                    ModelCount = 1,
                };
        }

        private static void AddVeinProtos(params VeinProto[] protos)
        {
            VeinProtoSet veins = LDB.veins;

            int dataArrayLength = veins.dataArray.Length;

            Array.Resize(ref veins.dataArray, dataArrayLength + protos.Length);

            for (var index = 0; index < dataArrayLength; ++index)
            {
                if (veins.dataArray[index].Name == "分形硅矿")
                {
                    veins.dataArray[index].Name = "莫桑石矿";
                    veins.dataArray[index].Description = "I莫桑石";
                }
                Debug.LogFormat("veins id {0} {1}, MinerBaseModelIndex{2} MiningEffect{3}", index, veins.dataArray[index].Name, veins.dataArray[index].MinerBaseModelIndex, veins.dataArray[index].MiningEffect);
                Debug.LogFormat("MinerCircleModelIndex {0} ModelIndex{1} MiningAudio {2} ModelCount{3}", veins.dataArray[index].MinerCircleModelIndex, veins.dataArray[index].ModelIndex, veins.dataArray[index].MiningAudio, veins.dataArray[index].ModelCount);
                Debug.LogFormat("CircleRadius {0} MiningItem {1} MiningTime {2}", veins.dataArray[index].CircleRadius, veins.dataArray[index].MiningItem, veins.dataArray[index].MiningTime);
                Debug.LogFormat("IconPath {0}", veins.dataArray[index].IconPath);
            }

            for (var index = 0; index < protos.Length; ++index)
            {
                if (protos[index].ID == 16 || protos[index].ID == 19)
                {
                    protos[index].MinerBaseModelIndex = 0;
                    protos[index].MinerCircleModelIndex = 0;
                    protos[index].CircleRadius = 1.5f;
                }
                veins.dataArray[dataArrayLength + index] = protos[index];
            }

            veins.OnAfterDeserialize();
        }

        internal static void SetMinerMk2Color()
        {
            Texture texture = Resources.Load<Texture>("Assets/texpack/矿机渲染索引");
            int veinColorTex = Shader.PropertyToID("_VeinColorTex");

            ref PrefabDesc prefabDesc = ref LDB.models.Select(256).prefabDesc;
            prefabDesc.materials[0].SetTexture(veinColorTex, texture);
            ref Material[] prefabDescLODMaterial = ref prefabDesc.lodMaterials[0];
            prefabDescLODMaterial[0].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[1].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[2].SetTexture(veinColorTex, texture);

            prefabDesc = ref LDB.models.Select(59).prefabDesc;
            prefabDescLODMaterial = ref prefabDesc.lodMaterials[0];
            prefabDescLODMaterial[1].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[2].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[3].SetTexture(veinColorTex, texture);
        }
        [HarmonyPatch(typeof(UISandboxMenu), nameof(UISandboxMenu.StaticLoad))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UISandboxMenu_StaticLoad_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(LDB), nameof(LDB.veins))), new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldlen));

            matcher.Advance(1).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)14))
               .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop)).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
               .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));

            return matcher.InstructionEnumeration();
        }


        [HarmonyPatch(typeof(UISandboxMenu), nameof(UISandboxMenu.StaticLoad))]
        [HarmonyPostfix]
        public static void UISandboxMenu_StaticLoad_Postfix(ref VeinProto[,] ___veinProtos)
        {
            ___veinProtos[1, 7] = LDB.veins.Select(15);
            ___veinProtos[1, 8] = LDB.veins.Select(16);
        }



        /*
        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_RemoveHeightLimit_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlanetData), nameof(PlanetData.radius))),
                new CodeMatch
                {
                    opcodes = new List<OpCode>
                    {
                        OpCodes.Blt, OpCodes.Blt_S,
                    },
                });

            CodeMatcher matcher2 = matcher.Clone();
            matcher2.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_0), new CodeMatch(OpCodes.Stloc_S));
            Label label = matcher2.Labels.First();

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Pop));
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Pop));
            matcher.SetAndAdvance(OpCodes.Br, label);

            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlanetData), nameof(PlanetData.waterItemId))));
            matcher.Advance(1).SetOpcodeAndAdvance(OpCodes.Br);

            return matcher.InstructionEnumeration();
        }
        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_ResizeVeinList_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)15));
            matcher.SetOperandAndAdvance(19);

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_RemoveVeinPositionBias_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop)).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
               .SetOpcodeAndAdvance(OpCodes.Br_S);

            return matcher.InstructionEnumeration();
        }


        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_InitialVeins_Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            var matcher = new CodeMatcher(instructions);

            Type type = original.DeclaringType;

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Stfld, AccessTools.Field(type, "veinVectorCount")));

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddVeinPatches), nameof(InitBirthVeinVectors))));

            matcher.Advance(1).SetOpcodeAndAdvance(OpCodes.Ldc_I4_5);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_2));

            matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_0);

            return matcher.InstructionEnumeration();
        }

        public static void InitBirthVeinVectors(PlanetAlgorithm algorithm)
        {
            //algorithm.veinVectorTypes[2] = EVeinType.Aluminum;
            //algorithm.veinVectors[2] = algorithm.planet.birthResourcePoint2;
            algorithm.veinVectorTypes[3] = EVeinType.Coal;
            algorithm.veinVectors[3] = algorithm.planet.birthResourcePoint3;
            algorithm.veinVectorTypes[4] = EVeinType.Stone;
            algorithm.veinVectors[4] = algorithm.planet.birthResourcePoint4;
        }


        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.GenBirthPoints), typeof(PlanetRawData), typeof(int))]
        [HarmonyPostfix]
        public static void PlanetData_GenBirthPoints_Postfix(PlanetData __instance, PlanetRawData rawData, int _birthSeed)
        {
            var dotNet35Random = new DotNet35Random(_birthSeed);
            Pose pose = __instance.PredictPose(85.0);
            Vector3 vector3 = Maths.QInvRotateLF(pose.rotation, __instance.star.uPosition - (VectorLF3)pose.position * 40000.0);
            vector3.Normalize();
            Vector3 x_direction = Vector3.Cross(vector3, Vector3.up).normalized;
            Vector3 y_direction = Vector3.Cross(x_direction, vector3).normalized;
            var num1 = 0;
            const int num2 = 512;

            for (; num1 < num2; ++num1)
            {
                float num3 = (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.5f;
                float num4 = (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.5f;
                Vector3 random = vector3 + num3 * x_direction + num4 * y_direction;
                random.Normalize();
                __instance.birthPoint = random * (float)(__instance.realRadius + 0.20000000298023224 + 1.4500000476837158);
                var tmpVector3 = Vector3.Cross(random, Vector3.up);
                x_direction = tmpVector3.normalized;
                tmpVector3 = Vector3.Cross(x_direction, random);
                y_direction = tmpVector3.normalized;

                for (var index = 0; index < 10; ++index)
                {
                    Vector2 rotate_0 = new Vector2((float)(dotNet35Random.NextDouble() * 2.0 - 1.0),
                        (float)(dotNet35Random.NextDouble() * 2.0 - 1.0)).normalized * 0.1f;
                    Vector2 rotate_1 = Rotate(rotate_0, 120);
                    Modify(dotNet35Random, ref rotate_1);
                    Vector2 rotate_2 = Rotate(rotate_0, 240);
                    Modify(dotNet35Random, ref rotate_2);
                    Vector2 rotate_3 = Rotate(rotate_0, 60);
                    Modify(dotNet35Random, ref rotate_3);

                    tmpVector3 = random + rotate_0.x * x_direction + rotate_0.y * y_direction;
                    __instance.birthResourcePoint0 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_1.x * x_direction + rotate_1.y * y_direction;
                    __instance.birthResourcePoint1 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_2.x * x_direction + rotate_2.y * y_direction;
                    __instance.birthResourcePoint2 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_0.x * -2 * x_direction + rotate_0.y * -2 * y_direction;
                    __instance.birthResourcePoint3 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_3.x * 2 * x_direction + rotate_3.y * 2 * y_direction;
                    __instance.birthResourcePoint4 = tmpVector3.normalized;

                    if (QueryHeightsNear(rawData, x_direction, y_direction, __instance.realRadius, random, __instance.birthResourcePoint0,
                        __instance.birthResourcePoint1, __instance.birthResourcePoint2, __instance.birthResourcePoint3,
                        __instance.birthResourcePoint4))
                        return;
                }
            }
        }

        */
        internal static Vector2 Rotate(Vector2 v, float angle)
        {
            float delta = angle * Mathf.PI / 180;

            return new Vector2(v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta), v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta));
        }
        private static void Modify(DotNet35Random dotNet35Random, ref Vector2 vector)
        {
            vector.x += (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.006f;
            vector.y += (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.006f;
        }

        private static bool QueryHeights(PlanetRawData rawData, float radius, params Vector3[] points)
        {
            return points.All(point => rawData.QueryHeight(point) > radius);
        }

        private static bool QueryHeightsNear(PlanetRawData rawData, Vector3 x_vector, Vector3 y_vector, float radius,
            params Vector3[] points)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Vector3 point in points)
            {
                Vector3 pos1 = point + x_vector * 0.03f;
                Vector3 pos2 = point - x_vector * 0.03f;
                Vector3 pos3 = point + y_vector * 0.03f;
                Vector3 pos4 = point - y_vector * 0.03f;

                if (!QueryHeights(rawData, radius, point, pos1, pos2, pos3, pos4)) return false;
            }

            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static void CheckBuildConditionsPatch(BuildTool_Click __instance, ref bool __result)
        {
            for (int i = 0; i < __instance.buildPreviews.Count; i++)
            {
                BuildPreview buildPreview = __instance.buildPreviews[i];
                Vector3 vector2 = buildPreview.lpos;
                Quaternion quaternion = buildPreview.lrot;
                Pose lPose = new Pose(buildPreview.lpos, buildPreview.lrot);
                Vector3 forward = lPose.forward;
                Vector3 up = lPose.up;
                if (buildPreview.desc.veinMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    PrebuildData prebuildData = default(PrebuildData);
                    int paramCount = 0;
                    if (buildPreview.desc.isVeinCollector)
                    {
                        Vector3 center = vector2.normalized * __instance.controller.cmd.test.magnitude + forward * -10f;
                        int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 18f, ref BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                        VeinData[] veinPool = __instance.factory.veinPool;
                        EVeinType eVeinType = EVeinType.None;
                        for (int j = 0; j < veinsInAreaNonAlloc; j++)
                        {
                            if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
                            {
                                if (veinPool[BuildTool._tmp_ids[j]].type == EVeinType.Oil || veinPool[BuildTool._tmp_ids[j]].type == EVeinType.Ice || veinPool[BuildTool._tmp_ids[j]].type == EVeinType.DeepMagma || !MinerComponent.IsTargetVeinInRange(veinPool[BuildTool._tmp_ids[j]].pos, lPose, buildPreview.desc))
                                {
                                    continue;
                                }

                                if (eVeinType != veinPool[BuildTool._tmp_ids[j]].type)
                                {
                                    if (eVeinType == EVeinType.None)
                                    {
                                        eVeinType = veinPool[BuildTool._tmp_ids[j]].type;
                                    }
                                    else
                                    {
                                        buildPreview.condition = EBuildCondition.NeedSingleResource;
                                    }
                                }

                                prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[j];
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }
                    else
                    {
                        Vector3 center2 = vector2.normalized * __instance.controller.cmd.test.magnitude + forward * -1.2f;
                        int veinsInAreaNonAlloc2 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center2, 12f, ref BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc2);
                        VeinData[] veinPool2 = __instance.factory.veinPool;
                        EVeinType eVeinType2 = EVeinType.None;
                        for (int k = 0; k < veinsInAreaNonAlloc2; k++)
                        {
                            if (BuildTool._tmp_ids[k] != 0 && veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k])
                            {
                                if (veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Oil || veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Ice || veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.DeepMagma || !MinerComponent.IsTargetVeinInRange(veinPool2[BuildTool._tmp_ids[k]].pos, lPose, buildPreview.desc))
                                {
                                    continue;
                                }

                                if (eVeinType2 != veinPool2[BuildTool._tmp_ids[k]].type)
                                {
                                    if (eVeinType2 == EVeinType.None)
                                    {
                                        eVeinType2 = veinPool2[BuildTool._tmp_ids[k]].type;
                                    }
                                    else
                                    {
                                        buildPreview.condition = EBuildCondition.NeedResource;
                                    }
                                }

                                prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[k];
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }

                    prebuildData.paramCount = paramCount;
                    prebuildData.ArrangeParametersArray();
                    if (buildPreview.desc.isVeinCollector)
                    {
                        if (buildPreview.paramCount == 0)
                        {
                            buildPreview.parameters = new int[2048];
                            buildPreview.paramCount = 2048;
                        }

                        if (prebuildData.paramCount > 0)
                        {
                            Array.Resize(ref buildPreview.parameters, buildPreview.paramCount + prebuildData.paramCount);
                            Array.Copy(prebuildData.parameters, 0, buildPreview.parameters, buildPreview.paramCount, prebuildData.paramCount);
                            buildPreview.paramCount += prebuildData.paramCount;
                        }
                    }
                    else
                    {
                        buildPreview.parameters = prebuildData.parameters;
                        buildPreview.paramCount = prebuildData.paramCount;
                    }

                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    if (prebuildData.paramCount == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }
                }
                else if (buildPreview.desc.oilMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    Vector3 vector4 = vector2;
                    Vector3 vector5 = -up;
                    int veinsInAreaNonAlloc3 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector4, 10f, ref BuildTool._tmp_ids);
                    PrebuildData prebuildData2 = default(PrebuildData);
                    prebuildData2.isDestroyed = false;
                    prebuildData2.InitParametersArray(veinsInAreaNonAlloc3);
                    VeinData[] veinPool3 = __instance.factory.veinPool;
                    int num2 = 0;
                    float num3 = 100f;
                    Vector3 pos = vector4;
                    for (int l = 0; l < veinsInAreaNonAlloc3; l++)
                    {
                        if (BuildTool._tmp_ids[l] != 0 && veinPool3[BuildTool._tmp_ids[l]].id == BuildTool._tmp_ids[l] && (veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.Oil || veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.Ice || veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.DeepMagma))
                        {
                            Vector3 pos2 = veinPool3[BuildTool._tmp_ids[l]].pos;
                            Vector3 vector6 = pos2 - vector4;
                            float num4 = Vector3.Dot(vector5, vector6);
                            float sqrMagnitude = (vector6 - vector5 * num4).sqrMagnitude;
                            if (sqrMagnitude < num3)
                            {
                                num3 = sqrMagnitude;
                                num2 = BuildTool._tmp_ids[l];
                                pos = pos2;
                                buildPreview.condition = EBuildCondition.Ok;
                            }
                        }
                    }

                    if (num2 == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }

                    prebuildData2.parameters[0] = num2;
                    prebuildData2.paramCount = 1;
                    prebuildData2.ArrangeParametersArray();
                    buildPreview.parameters = prebuildData2.parameters;
                    buildPreview.paramCount = prebuildData2.paramCount;
                    Vector3 vector7 = __instance.factory.planet.aux.Snap(pos, onTerrain: true);
                    vector2 = (lPose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector7)));
                    quaternion = (lPose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector7, __instance.yaw))));
                    forward = lPose.forward;
                    up = lPose.up;
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                }
            }

            __result = true;
            for (int num86 = 0; num86 < __instance.buildPreviews.Count; num86++)
            {
                BuildPreview buildPreview4 = __instance.buildPreviews[num86];
                if (buildPreview4.condition != 0 && buildPreview4.condition != EBuildCondition.NeedConn)
                {
                    __result = false;
                    __instance.actionBuild.model.cursorState = -1;
                    __instance.actionBuild.model.cursorText = buildPreview4.conditionText;
                    break;
                }
            }
            if (__result)
            {
                __instance.actionBuild.model.cursorState = 0;
                __instance.actionBuild.model.cursorText = BuildPreview.GetConditionText(EBuildCondition.Ok);
            }
        }


        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyPrefix]
        public static bool PlanetAlgorithm_GenerateVeins_Prefix(PlanetAlgorithm __instance)
        {
            lock (__instance.planet)
            {
                ThemeProto themeProto = LDB.themes.Select(__instance.planet.theme);
                if (themeProto == null)
                {
                    return true;
                }

                DotNet35Random dotNet35Random = new DotNet35Random(__instance.planet.seed);
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                int birthSeed = dotNet35Random.Next();
                DotNet35Random dotNet35Random2 = new DotNet35Random(dotNet35Random.Next());
                PlanetRawData data = __instance.planet.data;
                float num = 2.1f / __instance.planet.radius;
                VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
                int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
                int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
                int[] veinProducts = PlanetModelingManager.veinProducts;
                int[] array = new int[veinProtos.Length];
                float[] array2 = new float[veinProtos.Length];
                float[] array3 = new float[veinProtos.Length];
                if (themeProto.VeinSpot != null)
                {
                    Array.Copy(themeProto.VeinSpot, 0, array, 1, Math.Min(themeProto.VeinSpot.Length, array.Length - 1));
                }

                if (themeProto.VeinCount != null)
                {
                    Array.Copy(themeProto.VeinCount, 0, array2, 1, Math.Min(themeProto.VeinCount.Length, array2.Length - 1));
                }

                if (themeProto.VeinOpacity != null)
                {
                    Array.Copy(themeProto.VeinOpacity, 0, array3, 1, Math.Min(themeProto.VeinOpacity.Length, array3.Length - 1));
                }

                float p = 1f;
                ESpectrType spectr = __instance.planet.star.spectr;
                switch (__instance.planet.star.type)
                {
                    case EStarType.MainSeqStar:
                        switch (spectr)
                        {
                            case ESpectrType.M:
                                p = 2.5f;
                                break;
                            case ESpectrType.K:
                                p = 1f;
                                break;
                            case ESpectrType.G:
                                p = 0.7f;
                                break;
                            case ESpectrType.F:
                                p = 0.6f;
                                break;
                            case ESpectrType.A:
                                p = 1f;
                                break;
                            case ESpectrType.B:
                                p = 0.4f;
                                break;
                            case ESpectrType.O:
                                p = 1.6f;
                                break;
                        }

                        break;
                    case EStarType.GiantStar:
                        p = 2.5f;
                        break;
                    case EStarType.WhiteDwarf:
                        {
                            p = 3.5f;
                            array[9]++;
                            array[9]++;
                            for (int j = 1; j < 12; j++)
                            {
                                if (dotNet35Random.NextDouble() >= 0.44999998807907104)
                                {
                                    break;
                                }

                                array[9]++;
                            }

                            array2[9] = 0.7f;
                            array3[9] = 1f;
                            array[10]++;
                            array[10]++;
                            for (int k = 1; k < 12; k++)
                            {
                                if (dotNet35Random.NextDouble() >= 0.44999998807907104)
                                {
                                    break;
                                }

                                array[10]++;
                            }

                            array2[10] = 0.7f;
                            array3[10] = 1f;
                            array[12]++;
                            for (int l = 1; l < 12; l++)
                            {
                                if (dotNet35Random.NextDouble() >= 0.5)
                                {
                                    break;
                                }

                                array[12]++;
                            }

                            array2[12] = 0.7f;
                            array3[12] = 0.3f;
                            break;
                        }
                    case EStarType.NeutronStar:
                        {
                            p = 4.5f;
                            array[14]++;
                            for (int m = 1; m < 12; m++)
                            {
                                if (dotNet35Random.NextDouble() >= 0.64999997615814209)
                                {
                                    break;
                                }

                                array[14]++;
                            }

                            array2[14] = 0.7f;
                            array3[14] = 0.3f;
                            break;
                        }
                    case EStarType.BlackHole:
                        {
                            p = 5f;
                            array[14]++;
                            for (int i = 1; i < 12; i++)
                            {
                                if (dotNet35Random.NextDouble() >= 0.64999997615814209)
                                {
                                    break;
                                }

                                array[14]++;
                            }

                            array2[14] = 0.7f;
                            array3[14] = 0.3f;
                            break;
                        }
                }

                for (int n = 0; n < themeProto.RareVeins.Length; n++)
                {
                    int num2 = themeProto.RareVeins[n];
                    float num3 = ((__instance.planet.star.index == 0) ? themeProto.RareSettings[n * 4] : themeProto.RareSettings[n * 4 + 1]);
                    float num4 = themeProto.RareSettings[n * 4 + 2];
                    float num5 = themeProto.RareSettings[n * 4 + 3];
                    float num6 = num5;
                    num3 = 1f - Mathf.Pow(1f - num3, p);
                    num5 = 1f - Mathf.Pow(1f - num5, p);
                    num6 = 1f - Mathf.Pow(1f - num6, p);
                    if (!(dotNet35Random.NextDouble() < (double)num3))
                    {
                        continue;
                    }

                    array[num2]++;
                    array2[num2] = num5;
                    array3[num2] = num5;
                    for (int num7 = 1; num7 < 12; num7++)
                    {
                        if (dotNet35Random.NextDouble() >= (double)num4)
                        {
                            break;
                        }

                        array[num2]++;
                    }
                }

                //star id是1，star index是0
                if (__instance.planet.star.id == 1)
                {
                    if (__instance.planet.index == 3)
                    {
                        array[8] = 7;
                        array2[8] = 0.4f;
                        array3[8] = 0.8f;

                        array[17] = 4;
                        array2[17] = 0.2f;
                        array3[17] = 0.5f;

                        array[18] = 3;
                        array2[18] = 0.2f;
                        array3[18] = 0.5f;

                        array[19] = 12;
                        array2[19] = 1.0f;
                        array3[19] = 1.0f;
                    }
                    if (__instance.planet.index == 0)
                    {
                        array[17] = 4;
                        array2[17] = 0.3f;
                        array3[17] = 0.7f;
                    }
                }

                bool flag = __instance.planet.galaxy.birthPlanetId == __instance.planet.id;
                if (flag)
                {
                    __instance.planet.GenBirthPoints(data, birthSeed);
                }

                float num8 = __instance.planet.star.resourceCoef;
                bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
                bool isRareResource = GameMain.data.gameDesc.isRareResource;
                if (flag)
                {
                    num8 *= 2f / 3f;
                }
                else if (isRareResource)
                {
                    if (num8 > 1f)
                    {
                        num8 = Mathf.Pow(num8, 0.8f);
                    }

                    num8 *= 0.7f;
                }

                float num9 = 1f;
                num9 *= 1.1f;
                Array.Clear(__instance.veinVectors, 0, __instance.veinVectors.Length);
                Array.Clear(__instance.veinVectorTypes, 0, __instance.veinVectorTypes.Length);
                __instance.veinVectorCount = 0;
                Vector3 birthPoint;
                if (flag)
                {
                    birthPoint = __instance.planet.birthPoint;
                    birthPoint.Normalize();
                    birthPoint *= 0.75f;
                }
                else
                {
                    birthPoint.x = (float)dotNet35Random2.NextDouble() * 2f - 1f;
                    birthPoint.y = (float)dotNet35Random2.NextDouble() - 0.5f;
                    birthPoint.z = (float)dotNet35Random2.NextDouble() * 2f - 1f;
                    birthPoint.Normalize();
                    birthPoint *= (float)(dotNet35Random2.NextDouble() * 0.4 + 0.2);
                }

                __instance.planet.veinBiasVector = birthPoint;
                if (flag)
                {
                    __instance.veinVectorTypes[0] = EVeinType.Iron;
                    __instance.veinVectors[0] = __instance.planet.birthResourcePoint0;
                    __instance.veinVectorTypes[1] = EVeinType.Copper;
                    __instance.veinVectors[1] = __instance.planet.birthResourcePoint1;
                    __instance.veinVectorCount = 2;
                }

                for (int num10 = 1; num10 < 20; num10++)
                {
                    if (__instance.veinVectorCount >= __instance.veinVectors.Length)
                    {
                        break;
                    }

                    EVeinType eVeinType = (EVeinType)num10;
                    int num11 = array[num10];
                    if (num11 > 1)
                    {
                        num11 += dotNet35Random2.Next(-1, 2);
                    }

                    for (int num12 = 0; num12 < num11; num12++)
                    {
                        int num13 = 0;
                        Vector3 zero = Vector3.zero;
                        bool flag2 = false;
                        while (num13++ < 200)
                        {
                            zero.x = (float)dotNet35Random2.NextDouble() * 2f - 1f;
                            zero.y = (float)dotNet35Random2.NextDouble() * 2f - 1f;
                            zero.z = (float)dotNet35Random2.NextDouble() * 2f - 1f;
                            if (eVeinType != EVeinType.Oil || eVeinType != EVeinType.Ice || eVeinType != EVeinType.DeepMagma)
                            {
                                zero += birthPoint;
                            }

                            zero.Normalize();
                            float num14 = data.QueryHeight(zero);
                            if (num14 < __instance.planet.radius || ((eVeinType == EVeinType.Oil || eVeinType == EVeinType.Ice || eVeinType == EVeinType.DeepMagma) && num14 < __instance.planet.radius + 0.5f))
                            {
                                continue;
                            }

                            bool flag3 = false;
                            float num15 = ((eVeinType == EVeinType.Oil || eVeinType == EVeinType.Ice || eVeinType == EVeinType.DeepMagma) ? 100f : 196f);
                            for (int num16 = 0; num16 < __instance.veinVectorCount; num16++)
                            {
                                if ((__instance.veinVectors[num16] - zero).sqrMagnitude < num * num * num15)
                                {
                                    flag3 = true;
                                    break;
                                }
                            }

                            if (!flag3)
                            {
                                flag2 = true;
                                break;
                            }
                        }

                        if (flag2)
                        {
                            __instance.veinVectors[__instance.veinVectorCount] = zero;
                            __instance.veinVectorTypes[__instance.veinVectorCount] = eVeinType;
                            __instance.veinVectorCount++;
                            if (__instance.veinVectorCount == __instance.veinVectors.Length)
                            {
                                break;
                            }
                        }
                    }
                }

                data.veinCursor = 1;
                __instance.tmp_vecs.Clear();
                VeinData vein = default(VeinData);
                for (int num17 = 0; num17 < __instance.veinVectorCount; num17++)
                {
                    __instance.tmp_vecs.Clear();
                    Vector3 normalized = __instance.veinVectors[num17].normalized;
                    EVeinType eVeinType2 = __instance.veinVectorTypes[num17];
                    int num18 = (int)eVeinType2;
                    Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                    Vector3 vector = quaternion * Vector3.right;
                    Vector3 vector2 = quaternion * Vector3.forward;
                    __instance.tmp_vecs.Add(Vector2.zero);
                    int num19 = Mathf.RoundToInt(array2[num18] * (float)dotNet35Random2.Next(20, 25));
                    if (eVeinType2 == EVeinType.Oil || eVeinType2 == EVeinType.Ice || eVeinType2 == EVeinType.DeepMagma)
                    {
                        num19 = 1;
                    }

                    float num20 = array3[num18];
                    if (flag && num17 < 2)
                    {
                        num19 = 6;
                        num20 = 0.2f;
                    }

                    int num21 = 0;
                    while (num21++ < 20)
                    {
                        int count = __instance.tmp_vecs.Count;
                        for (int num22 = 0; num22 < count; num22++)
                        {
                            if (__instance.tmp_vecs.Count >= num19)
                            {
                                break;
                            }

                            if (__instance.tmp_vecs[num22].sqrMagnitude > 36f)
                            {
                                continue;
                            }

                            double num23 = dotNet35Random2.NextDouble() * Math.PI * 2.0;
                            Vector2 vector3 = new Vector2((float)Math.Cos(num23), (float)Math.Sin(num23));
                            vector3 += __instance.tmp_vecs[num22] * 0.2f;
                            vector3.Normalize();
                            Vector2 vector4 = __instance.tmp_vecs[num22] + vector3;
                            bool flag4 = false;
                            for (int num24 = 0; num24 < __instance.tmp_vecs.Count; num24++)
                            {
                                if ((__instance.tmp_vecs[num24] - vector4).sqrMagnitude < 0.85f)
                                {
                                    flag4 = true;
                                    break;
                                }
                            }

                            if (!flag4)
                            {
                                __instance.tmp_vecs.Add(vector4);
                            }
                        }

                        if (__instance.tmp_vecs.Count >= num19)
                        {
                            break;
                        }
                    }

                    float num25 = num8;
                    if (eVeinType2 == EVeinType.Oil || eVeinType2 == EVeinType.Ice || eVeinType2 == EVeinType.DeepMagma)
                    {
                        num25 = Mathf.Pow(num8, 0.5f);
                    }

                    int num26 = Mathf.RoundToInt(num20 * 100000f * num25);
                    if (num26 < 20)
                    {
                        num26 = 20;
                    }

                    int num27 = ((num26 < 16000) ? Mathf.FloorToInt((float)num26 * 0.9375f) : 15000);
                    int minValue = num26 - num27;
                    int maxValue = num26 + num27 + 1;
                    for (int num28 = 0; num28 < __instance.tmp_vecs.Count; num28++)
                    {
                        Vector3 vector5 = (__instance.tmp_vecs[num28].x * vector + __instance.tmp_vecs[num28].y * vector2) * num;
                        vein.type = eVeinType2;
                        vein.groupIndex = (short)(num17 + 1);
                        vein.modelIndex = (short)dotNet35Random2.Next(veinModelIndexs[num18], veinModelIndexs[num18] + veinModelCounts[num18]);
                        vein.amount = Mathf.RoundToInt((float)dotNet35Random2.Next(minValue, maxValue) * num9);
                        if (eVeinType2 != EVeinType.Oil || eVeinType2 != EVeinType.Ice || eVeinType2 != EVeinType.DeepMagma)
                        {
                            vein.amount = Mathf.RoundToInt((float)vein.amount * DSPGame.GameDesc.resourceMultiplier);
                        }
                        else
                        {
                            vein.amount = Mathf.RoundToInt((float)vein.amount * DSPGame.GameDesc.oilAmountMultiplier);
                        }

                        if (vein.amount < 1)
                        {
                            vein.amount = 1;
                        }

                        if (isInfiniteResource && (vein.type != EVeinType.Oil || vein.type != EVeinType.Ice || vein.type != EVeinType.DeepMagma))
                        {
                            vein.amount = 1000000000;
                        }

                        vein.productId = veinProducts[num18];
                        vein.pos = normalized + vector5;
                        if (vein.type == EVeinType.Oil || vein.type == EVeinType.Ice || vein.type == EVeinType.DeepMagma)
                        {
                            vein.pos = __instance.planet.aux.RawSnap(vein.pos);
                        }

                        vein.minerCount = 0;
                        float num29 = data.QueryHeight(vein.pos);
                        data.EraseVegetableAtPoint(vein.pos);
                        vein.pos = vein.pos.normalized * num29;
                        if (__instance.planet.waterItemId == 0 || !(num29 < __instance.planet.radius))
                        {
                            data.AddVeinData(vein);
                        }
                    }
                }

                __instance.tmp_vecs.Clear();
                return false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVeinDetailNode), "_OnUpdate")]
        public static void _OnUpdatePatch(ref UIVeinDetailNode __instance)
        {
            VeinGroup veinGroup = __instance.inspectFactory.veinGroups[__instance.veinGroupIndex];
            if (veinGroup.count == 0 || veinGroup.type == EVeinType.None)
            {
                __instance._Close();
                return;
            }

            if (__instance.counter % 4 == 0 && __instance.showingAmount != veinGroup.amount)
            {
                __instance.showingAmount = veinGroup.amount;
                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + __instance.veinProto.name + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVeinDetailNode), "Refresh")]
        public static void RefreshPatch(ref UIVeinDetailNode __instance)
        {
            if (__instance.inspectFactory == null)
            {
                return;
            }

            if (__instance.veinGroupIndex >= __instance.inspectFactory.veinGroups.Length)
            {
                __instance._Close();
                return;
            }

            VeinGroup veinGroup = __instance.inspectFactory.veinGroups[__instance.veinGroupIndex];
            if (veinGroup.count == 0 || veinGroup.type == EVeinType.None)
            {
                __instance._Close();
                return;
            }

            __instance.veinProto = LDB.veins.Select((int)veinGroup.type);
            if (__instance.veinProto != null)
            {
                __instance.veinIcon.sprite = __instance.veinProto.iconSprite;
                __instance.showingAmount = veinGroup.amount;
                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + __instance.veinProto.name + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                    return;
                }
            }
            else
            {
                __instance.veinIcon.sprite = null;
                __instance.showingAmount = veinGroup.amount;
                if (__instance.menuButton != null)
                {
                    __instance.menuButton.gameObject.SetActive(value: false);
                }

                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + " ? " + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIResAmountEntry), "SetInfo")]
        public static void SetInfoPatch(ref UIResAmountEntry __instance, string label, ref string strBuilderFormat)
        {
            if (label.Equals("深层岩浆") || label.Equals("水"))
            {
                strBuilderFormat = "         /s";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPlanetDetail), "RefreshDynamicProperties")]
        public static bool RefreshDynamicPropertiesPatch(ref UIPlanetDetail __instance)
        {
            if (__instance.veinAmounts == null)
            {
                __instance.veinAmounts = new long[64];
            }

            if (__instance.veinCounts == null)
            {
                __instance.veinCounts = new int[64];
            }

            Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
            Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
            bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
            if (__instance.planet == null)
            {
                return true;
            }

            if (!__instance._scanned && __instance.planet.scanned)
            {
                __instance.OnPlanetDataSet();
                return true;
            }

            __instance._scanned = __instance.planet.scanned;
            int num = ((__instance.planet == GameMain.localPlanet) ? 1 : ((__instance.planet.star == GameMain.localStar) ? 2 : (((GameMain.mainPlayer.uPosition - __instance.planet.uPosition).magnitude < 14400000.0) ? 3 : 4)));
            bool flag = GameMain.history.universeObserveLevel >= num;
            if (__instance.planet.factory != null && GameMain.history.universeObserveLevel >= 1)
            {
                flag = true;
            }

            if (__instance._scanned && flag)
            {
                __instance.planet.SummarizeVeinAmountsByFilter(ref __instance.veinAmounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
                __instance.planet.SummarizeVeinCountsByFilter(ref __instance.veinCounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
            }

            foreach (UIResAmountEntry entry in __instance.entries)
            {
                if (entry.refId <= 0)
                {
                    continue;
                }

                if (flag)
                {
                    if (entry.refId == 7 || entry.refId == (int)EVeinType.DeepMagma || entry.refId == (int)EVeinType.Ice)
                    {
                        double num2 = (double)__instance.veinAmounts[entry.refId] * (double)VeinData.oilSpeedMultiplier;
                        if (__instance.uiGame.veinAmountDisplayFilter == 1)
                        {
                            num2 *= (double)GameMain.history.miningSpeedScale;
                        }

                        StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 8, (float)num2);
                        entry.DisplayStringBuilder();
                    }
                    else
                    {
                        long num3 = __instance.veinAmounts[entry.refId];
                        int num4 = __instance.veinCounts[entry.refId];
                        if (isInfiniteResource)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num4);
                        }
                        else if (num3 < 1000000000)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num3);
                        }
                        else
                        {
                            StringBuilderUtility.WriteKMG(entry.sb, 15, num3);
                        }

                        entry.DisplayStringBuilder();
                    }

                    entry.SetObserved(_observed: true);
                }
                else
                {
                    entry.valueString = "未知".Translate();
                    if (entry.refId > 7)
                    {
                        entry.overrideLabel = "未知珍奇信号".Translate();
                    }

                    if (entry.refId > 7)
                    {
                        entry.SetObserved(_observed: false);
                    }
                    else
                    {
                        entry.SetObserved(_observed: true);
                    }
                }
            }

            if (__instance.tipEntry != null)
            {
                if (!flag)
                {
                    __instance.tipEntry.valueString = "宇宙探索等级".Translate() + num;
                }
                else
                {
                    __instance.tipEntry.valueString = "";
                }

                __instance.SetResCount(flag ? (__instance.entries.Count - 1) : __instance.entries.Count);
            }
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "RefreshDynamicProperties")]
        public static bool UIStarDetailRefreshDynamicPropertiesPatch(ref UIStarDetail __instance)
        {
            bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
            if (__instance.veinAmounts == null)
            {
                __instance.veinAmounts = new long[64];
            }

            if (__instance.veinCounts == null)
            {
                __instance.veinCounts = new int[64];
            }

            Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
            Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
            if (__instance.star == null)
            {
                return true;
            }

            if (!__instance.calculated && __instance.star.scanned)
            {
                __instance.OnStarDataSet();
                return false;
            }

            __instance.calculated = __instance.star.scanned;
            bool num = __instance.observed;
            double magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
            int num2 = ((__instance.star == GameMain.localStar) ? 2 : ((magnitude < 14400000.0) ? 3 : 4));
            __instance.observed = GameMain.history.universeObserveLevel >= num2;
            if (num != __instance.observed)
            {
                __instance.OnStarDataSet();
                return true;
            }

            __instance.loadingTextGo.SetActive(__instance.observed && !__instance.calculated);
            if (__instance.calculated && __instance.observed)
            {
                __instance.star.CalcVeinAmounts(ref __instance.veinAmounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
                __instance.star.CalcVeinCounts(ref __instance.veinCounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
            }

            foreach (UIResAmountEntry entry in __instance.entries)
            {
                if (entry.refId <= 0)
                {
                    continue;
                }

                if (__instance.observed)
                {
                    long num3 = __instance.veinAmounts[entry.refId];
                    long value = __instance.veinCounts[entry.refId];
                    if (entry.refId == 7 || entry.refId == (int)EVeinType.DeepMagma || entry.refId == (int)EVeinType.Ice)
                    {
                        double num4 = (double)num3 * (double)VeinData.oilSpeedMultiplier;
                        if (__instance.uiGame.veinAmountDisplayFilter == 1)
                        {
                            num4 *= (double)GameMain.history.miningSpeedScale;
                        }

                        StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 8, (float)num4);
                        entry.DisplayStringBuilder();
                    }
                    else
                    {
                        if (isInfiniteResource)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)value);
                        }
                        else if (num3 < 1000000000)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num3);
                        }
                        else
                        {
                            StringBuilderUtility.WriteKMG(entry.sb, 15, num3);
                        }

                        entry.DisplayStringBuilder();
                    }

                    entry.SetObserved(_observed: true);
                }
                else
                {
                    entry.valueString = "未知".Translate();
                    if (entry.refId > 7)
                    {
                        entry.overrideLabel = "未知珍奇信号".Translate();
                    }

                    if (entry.refId > 7)
                    {
                        entry.SetObserved(_observed: false);
                    }
                    else
                    {
                        entry.SetObserved(_observed: true);
                    }
                }
            }

            if (__instance.tipEntry != null)
            {
                if (!__instance.observed)
                {
                    __instance.tipEntry.valueString = "宇宙探索等级".Translate() + num2;
                }
                else
                {
                    __instance.tipEntry.valueString = "";
                }

                __instance.SetResCount(__instance.observed ? (__instance.entries.Count - 1) : __instance.entries.Count);
            }
            return false;
        }
    }
}
