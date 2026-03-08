using System.Collections.Generic;
using System.Threading;
using GAS.AbilitySystem;
using GAS.Core;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Component
{
    /// <summary>
    /// 能力系统组件 - 挂载在角色身上
    /// 只管理技能（Abilities），GE 交给 GEManager
    /// </summary>
    public class AbilitySystemComponent : MonoBehaviour
    {
        [Header("组件依赖")]
        [LabelText("GE管理器"), SerializeField] GEManager geManager;
        [LabelText("属性控制器"), SerializeField] StatController statController;

        [Header("技能配置")]
        [LabelText("初始技能"), SerializeField] List<GameplayAbility> initialAbilities;

        //可激活的技能列表
        private readonly List<AbilitySpec> activatableAbilities = new();

        // CancellationToken
        private CancellationTokenSource _cts;

        void Awake()
        {
            statController ??= GetComponent<StatController>();
            _cts = new CancellationTokenSource();
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        /// <summary>
        /// 获取CancellationToken（用于UniTask）
        /// </summary>
        public CancellationToken GetCancellationTokenOnDestroy()
        {
            return _cts?.Token ?? default;
        }

        void Start()
        {
            // 初始化 GEManager
            if (geManager is not null)
            {
                geManager.SetStatController(statController);
            }

            //初始化技能配置
            foreach (var ability in initialAbilities)
            {
                var abilitySpec = ability.CreateAbilitySpec();
                activatableAbilities.Add(abilitySpec);
            }
        }

        void Update()
        {
            // 更新 GE
            geManager?.UpdateGE(Time.deltaTime);

            // 更新技能冷却
            foreach (var spec in activatableAbilities)
            {
                spec.cooldown?.UpdateCooldown(true, Time.deltaTime);
            }
        }

        #region 技能管理

        /// <summary>
        /// 尝试激活技能
        /// </summary>
        public bool TryActivateAbility(string abilityName, StatController stat)
        {
            Debug.Log($"[ASC] TryActivateAbility - abilityName: {abilityName}");
            
            //从激活列表中找到技能配置
            foreach (var spec in activatableAbilities)
            {
                Debug.Log($"[ASC] 检查技能: {spec.ability.abilityName}");
                
                //匹配技能名称 并且可以激活
                if (spec.ability.abilityName == abilityName)
                {
                    Debug.Log($"[ASC] 找到匹配技能: {abilityName}, 检查CanActivate...");
                    if (spec.CanActivate(stat))
                    {
                        Debug.Log($"[ASC] CanActivate通过，准备激活!");
                        spec.Activate(stat);
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"[ASC] CanActivate失败，技能无法激活");
                    }
                }
            }
            Debug.LogWarning($"[ASC] 未找到可激活的技能: {abilityName}");
            return false;
        }

        #endregion

        #region GE转发

        /// <summary>
        /// 应用GE（转发到GEManager）
        /// </summary>
        public GameplayEffectSpec ApplyGE(GameplayEffectData effectData, object source)
        {
            if (geManager is null)
            {
                Debug.LogWarning("GEManager 未配置，无法应用 GE");
                return null;
            }
            return geManager.ApplyGE(effectData, source);
        }

        /// <summary>
        /// 移除所有GE
        /// </summary>
        public void RemoveAllGE()
        {
            geManager?.RemoveAllGE();
        }

        #endregion
    }
}
