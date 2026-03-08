using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.Core;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.AbilitySystem
{
    /// <summary>
    /// 加速自身技能 - 持续时间内增加移动速度
    /// </summary>
    [CreateAssetMenu(fileName = "Ability_SpeedBoost", menuName = "GAS/Demo/Ability_SpeedBoost")]
    public class Ability_SpeedBoost : GameplayAbility
    {
        [Header("加速设置")]
        [SerializeField] private GameplayEffectData speedBoostEffect;
        [SerializeField] private GameObject speedEffectPrefab;

        public override void Activate(StatController statController)
        {
            Debug.Log($"[加速] 激活技能");

            if (speedBoostEffect == null)
            {
                Debug.LogWarning("[加速] 未配置加速GE!");
                return;
            }

            // 通过ASC应用到自身
            var asc = statController.GetComponent<AbilitySystemComponent>();
            if (asc == null)
            {
                Debug.LogWarning("[加速] 未找到AbilitySystemComponent!");
                return;
            }

            // 应用加速GE（持续效果）
            asc.ApplyGE(speedBoostEffect, asc.transform);

            // 生成加速特效
            if (speedEffectPrefab != null)
            {
                var owner = asc.transform;
                var spawnTask = Task_SpawnEffect.SpawnEffect(asc)
                    .SetEffectPrefab(speedEffectPrefab)
                    .SetTarget(owner)
                    .SetDuration(speedBoostEffect.DurationValue);
                spawnTask.Start();
            }

            Debug.Log($"[加速] 加速已应用，持续{speedBoostEffect.DurationValue}秒");
        }
    }
}
