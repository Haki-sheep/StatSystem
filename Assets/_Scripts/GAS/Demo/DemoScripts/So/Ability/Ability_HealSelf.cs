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
    /// 治疗自身技能 - 立即治疗自身
    /// </summary>
    [CreateAssetMenu(fileName = "Ability_HealSelf", menuName = "GAS/Demo/Ability_HealSelf")]
    public class Ability_HealSelf : GameplayAbility
    {
        [Header("治疗设置")]
        [SerializeField] private GameplayEffectData healEffect;
        [SerializeField] private GameObject healEffectPrefab;

        public override void Activate(StatController statController)
        {
            Debug.Log($"[治疗] 激活技能");

            if (healEffect == null)
            {
                Debug.LogWarning("[治疗] 未配置治疗GE!");
                return;
            }

            // 通过ASC应用到自身
            var asc = statController.GetComponent<AbilitySystemComponent>();
            if (asc == null)
            {
                Debug.LogWarning("[治疗] 未找到AbilitySystemComponent!");
                return;
            }

            // 应用治疗GE
            Debug.Log($"[治疗] 尝试应用GE: healEffect={healEffect}, source={asc.transform}");
            var spec = asc.ApplyGE(healEffect, asc.transform);
            Debug.Log($"[治疗] GE应用结果: spec={(spec != null ? "成功" : "失败")}");
            
            // 生成治疗特效
            if (healEffectPrefab != null)
            {
                var owner = asc.transform;
                var spawnTask = Task_SpawnEffect.SpawnEffect(asc)
                    .SetEffectPrefab(healEffectPrefab)
                    .SetLocation(owner.position)
                    .SetDuration(1f);
                spawnTask.Start();
            }

            Debug.Log($"[治疗] 治疗完成");
        }
    }
}
