using System;
using System.Collections.Generic;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// GE管理器 - 负责GameplayEffect的完整生命周期
    /// 包括：应用、堆叠、周期、持续时间、移除
    /// 注意：不持有 StatController，通过外部传入
    /// </summary>
    public class GEManager : MonoBehaviour
    {
        //现有GE表
        private List<GameplayEffectSpec> appliedEffectList = new();

        // 外部传入的 StatController
        private StatController statController;
        public void SetStatController(StatController sc) => statController = sc;

        #region 公开接口

        /// <summary>
        /// 应用GE
        /// </summary>
        public GameplayEffectSpec ApplyGE(GameplayEffectData effectData, object source)
        {
            if (effectData is null) { Debug.Log("GE为空"); return null; }
            if (statController is null) { Debug.Log("StatController 未设置"); return null; }

            Debug.Log($"[GEManager] ApplyGE: {effectData.name}, IsInstant={effectData.IsInstant}");

            //1.检查是否已经有相同的GE
            var existGe = FindMatchingEffect(effectData);

            if (existGe is not null)
            {
                return HandleStacking(existGe, effectData, source);
            }

            //2.不是堆叠时 创建新GE实例 
            GameplayEffectSpec newSpec = new GameplayEffectSpec(effectData, source);

            //3.即时效果：立即应用修饰符
            if (effectData.IsInstant)
            {
                Debug.Log("[GEManager] 即时效果，应用修饰符...");
                ApplyModifiersToStat(newSpec);
                return newSpec;
            }

            //4.持续效果：加入列表
            appliedEffectList.Add(newSpec);
            ApplyModifiersToStat(newSpec);

            return newSpec;
        }

        /// <summary>
        /// 移除所有GE（用于角色死亡等情况）
        /// </summary>
        public void RemoveAllGE()
        {
            for (int i = appliedEffectList.Count - 1; i >= 0; i--)
            {
                RemoveGameplayEffect(appliedEffectList[i], i);
            }
        }

        /// <summary>
        /// 每帧更新（由 ASC 调用）
        /// </summary>
        public void UpdateGE(float dt)
        {
            //倒叙
            for (int i = appliedEffectList.Count - 1; i >= 0; i--)
            {
                var spec = appliedEffectList[i];

                //1.减少持续时间
                if (spec.GEData.HasDuration)
                {
                    spec.RemainingDuration -= dt;

                    //如果到期
                    if (spec.IsExpired)
                    {
                        //移除修饰符
                        HandleExpiration(spec, i);
                        continue;
                    }
                }

                //2.处理周期效果
                if (spec.GEData.IsPeriodic)
                {
                    spec.RemainingPeriod -= dt;

                    //如果到期
                    if (spec.ShouldExecutePeriod)
                    {
                        //触发周期效果 重置时间
                        ApplyPeriodicEffect(spec);
                        spec.ResetPeriod();
                    }
                }
            }
        }

        #endregion

        #region GE生命周期

        /// <summary>
        /// 查找匹配的GE
        /// </summary>
        private GameplayEffectSpec FindMatchingEffect(GameplayEffectData effectData)
        {
            foreach (var ge in appliedEffectList)
            {
                if (ge.GEData == effectData)
                {
                    return ge;
                }
            }
            return null;
        }

        /// <summary>
        /// 处理GE堆叠
        /// </summary>
        private GameplayEffectSpec HandleStacking(GameplayEffectSpec existingSpec, GameplayEffectData effectData, object source)
        {
            //检查最大层
            if (existingSpec.StackCount >= effectData.StackLimit)
            {
                Debug.Log($"GE{effectData.name}已达到最大堆叠层数{effectData.StackLimit}");
                //即使已达上限，仍根据配置刷新持续时间和周期
                if (effectData.DurationRefresh == E_EffectDurationRefresh.RefreshOnSuccessfulApplication)
                {
                    existingSpec.RefreshDuration();
                }
                if (effectData.PeriodReset == E_EffectPeriodReset.ResetOnSuccessfulApplication)
                {
                    existingSpec.ResetPeriod();
                }
                return existingSpec;
            }

            //根据堆叠策略处理叠加表现
            switch (effectData.StackingPolicy)
            {
                case E_EffectStacking.StackUpper:
                    existingSpec.StackCount++;//层数 + 1
                    break;

                case E_EffectStacking.OverrideBySource:
                    //来源相同则刷新 不同层则 ++
                    if (existingSpec.Source == source)
                    {
                        //可刷新
                        if (existingSpec.DurationRefresh == E_EffectDurationRefresh.RefreshOnSuccessfulApplication)
                        {
                            existingSpec.RefreshDuration();
                        }
                    }
                    else
                    {
                        existingSpec.StackCount++;
                    }
                    break;

                case E_EffectStacking.SetToMaxStacks:
                    //直接设置最高层数
                    existingSpec.StackCount = effectData.StackLimit;
                    break;

                case E_EffectStacking.None:
                default:
                    //如果不允许堆叠 就直接刷新
                    if (existingSpec.DurationRefresh == E_EffectDurationRefresh.RefreshOnSuccessfulApplication)
                    {
                        existingSpec.RefreshDuration();
                    }
                    break;
            }
            // 周期重置 会出现新叠加层重置原有节奏
            // 这里默认有一种情况 就是周期不重置 也就是新层叠加不影响原有周期节奏
            if (effectData.PeriodReset == E_EffectPeriodReset.ResetOnSuccessfulApplication)
            {
                existingSpec.ResetPeriod();
            }

            //
            ApplyModifiersToStat(existingSpec, isRemove: true);
            ApplyModifiersToStat(existingSpec, isRemove: false);

            Debug.Log($"[GE] {effectData.name} 堆叠后层数: {existingSpec.StackCount}");
            return existingSpec;
        }

        /// <summary>
        /// 周期效果（Dot伤害）
        /// </summary>
        private void ApplyPeriodicEffect(GameplayEffectSpec spec)
        {
            //每次周期触发，应用单层修饰符
            var modifiers = spec.CreateModifiers(spec.Source);
            var configList = spec.GEData.StatModifierConfig;

            for (int i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];
                if (i >= configList.Count) continue;

                var config = configList[i];

                // 乘以层数
                float finalValue = mod.Value * spec.StackCount;

                // 直接修改属性值（Dot 总是修改即时属性）
                statController.ChangeAttributeValue(config.statId, finalValue, mod.ModifierType, spec.Source);
            }
            Debug.Log($"[GE] {spec.GEData.name} 周期触发，层数: {spec.StackCount}");
        }

        /// <summary>
        /// 处理GE到期
        /// </summary>
        private void HandleExpiration(GameplayEffectSpec spec, int i)
        {
            switch (spec.GEData.Expiration)
            {
                case E_EffectExpiration.ClearEntireStack:
                    //清除整个堆栈
                    RemoveGameplayEffect(spec, i);
                    break;

                case E_EffectExpiration.RemoveSingleStackAndRefreshDuration:
                    //移除单层并刷新持续时间
                    spec.StackCount--;
                    //如果还有层数
                    if (spec.StackCount > 0)
                    {
                        //刷新持续时间
                        spec.RefreshDuration();
                        //移除原有修饰符 再应用新的
                        ApplyModifiersToStat(spec, isRemove: true);
                        ApplyModifiersToStat(spec, isRemove: false);
                    }
                    else
                    {
                        //没有层数了 直接移除
                        RemoveGameplayEffect(spec, i);
                    }
                    break;
                case E_EffectExpiration.RefreshDuration:
                    //只刷新持续时间
                    spec.RefreshDuration();
                    break;

            }
        }

        /// <summary>
        /// 移除GE
        /// </summary>
        private void RemoveGameplayEffect(GameplayEffectSpec spec, int index)
        {
            appliedEffectList.RemoveAt(index);
            ApplyModifiersToStat(spec, isRemove: true);  // 移除修饰符
            Debug.Log($"[GE] {spec.GEData.name} 已移除");
        }

        #endregion

        #region 修饰符生命周期

        /// <summary>
        /// 立即应用修饰符
        /// </summary>
        private void ApplyModifiersToStat(GameplayEffectSpec spec, bool isRemove = false)
        {
            //创建修饰符列表
            var modifiers = spec.CreateModifiers(spec.Source);
            int stackCount = spec.StackCount;

            // 获取配置列表
            var configList = spec.GEData.StatModifierConfig;
            
            Debug.Log($"[GEManager] ApplyModifiersToStat: modifiers.Count={modifiers.Count}, configList.Count={configList.Count}");

            for (int i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];
                if (i >= configList.Count) 
                {
                    Debug.LogWarning($"[GEManager] 配置索引越界: i={i}, configList.Count={configList.Count}");
                    continue;
                }

                var config = configList[i];
                
                Debug.Log($"[GEManager] 处理修饰符: statId={config.statId}, type={config.type}, value={config.value}");

                if (isRemove)
                {
                    // 根据属性类型选择移除方式
                    var imStat = statController.GetImStat(config.statId);
                    if (imStat is not null)
                    {
                        // ImStat直接移除不需要 因为没有修饰符系统
                        continue;
                    }
                    else
                    {
                        // Passive 属性：移除修饰符
                        statController.RemoveModifiersFromSource(config.statId, spec.Source);
                    }
                }
                else
                {
                    //乘层数
                    float finalValue = mod.Value * spec.StackCount;

                    // 根据属性类型选择修改方式
                    Debug.Log($"[GEManager] GetImStat前: statId={config.statId}, StatDict.Keys={string.Join(", ", statController.StatDict.Keys)}");
                    var imStat = statController.GetImStat(config.statId);
                    Debug.Log($"[GEManager] GetImStat结果: imStat={imStat}");
                    if (imStat != null)
                    {
                        // ImStat：直接修改当前值
                        Debug.Log($"[GEManager] 修改ImStat: {config.statId}, value={finalValue}, type={mod.ModifierType}");
                        imStat.ChangeValue(finalValue, mod.ModifierType);
                    }
                    else
                    {
                        // Passive 属性：添加修饰符
                        Debug.Log($"[GEManager] 尝试添加修饰符到Passive属性: {config.statId}");
                        mod.Value = finalValue;
                        statController.AddModifier(config.statId, mod);
                    }
                }
            }
        }

        #endregion
    }
}
