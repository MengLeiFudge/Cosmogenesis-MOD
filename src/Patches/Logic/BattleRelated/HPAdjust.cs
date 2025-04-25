using ProjectGenesis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectGenesis.Patches.Logic.BattleRelated
{
    internal class HPAdjust
    {
        internal static void OurSideHPAdjust()
        {
            ModelProto modelProto = LDB.models.Select(374); // 机枪塔
            modelProto.HpMax = 80000;

            modelProto = LDB.models.Select(373); // 激光塔
            modelProto.HpMax = 70000;

            modelProto = LDB.models.Select(375); // 加农炮
            modelProto.HpMax = 20000;

            modelProto = LDB.models.Select(448); // 原型机
            modelProto.HpMax = 45000;
            modelProto.prefabDesc.craftUnitAttackDamage0 = 2000;
            modelProto = LDB.models.Select(449); // 攻击（原精准
            modelProto.HpMax = 75000;
            modelProto.prefabDesc.craftUnitAttackDamage0 = 2500;

            modelProto = LDB.models.Select(450); // 精准（原攻击
            modelProto.HpMax = 24000;
            modelProto.prefabDesc.craftUnitAttackDamage0 = 7500;
            modelProto.prefabDesc.craftUnitAttackRange0 = 85f;
            modelProto.prefabDesc.craftUnitSensorRange = 100f;
            modelProto.prefabDesc.craftUnitMaxMovementSpeed = 15f;
            
            modelProto = LDB.models.Select(451); // 护卫
            modelProto.HpMax = 250000;
            modelProto = LDB.models.Select(452); // 驱逐
            modelProto.HpMax = 1500000;

            modelProto = LDB.models.Select(482); // 地面电浆
            modelProto.HpMax = 120000;

        }


        internal static void ModifyEnemyHpUpgrade()
        {
            ModelProto model = LDB.models.Select(ProtoID.M导轨);
            model.HpMax *= 3;
            model.HpUpgrade *= 3;
            model.HpRecover *= 3;

            ModelProto modelProto = LDB.models.Select(ProtoID.M强袭者);
            modelProto.HpUpgrade = 2500;
            modelProto.prefabDesc.unitAttackRange0 = 10;

            modelProto = LDB.models.Select(ProtoID.M游骑兵);
            modelProto.HpUpgrade = 1000;
            modelProto.prefabDesc.unitAttackDamageInc0 = 250;

            modelProto = LDB.models.Select(ProtoID.M守卫者);
            modelProto.HpMax = 7500;
            modelProto.HpUpgrade = 1500;
            modelProto.prefabDesc.unitMaxMovementSpeed = 5;
            modelProto.prefabDesc.unitMarchMovementSpeed = 5;
            modelProto.prefabDesc.unitAttackDamageInc0 = 1500;
            modelProto.prefabDesc.unitAttackRange0 = 20;

            modelProto = LDB.models.Select(ProtoID.M高能激光塔);
            modelProto.prefabDesc.dfTurretAttackDamage = 26000;
            modelProto.prefabDesc.dfTurretAttackDamageInc = 3000;
        }
    }
}
