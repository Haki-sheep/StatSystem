using System.Collections.Generic;
using GAS.StateSystem;
namespace GAS.Core.GameplayEffect
{

    /// <summary>
    /// GameplayEffect 运行时实例
    /// 当一个GE被应用到目标时创建
    /// </summary>
    public class GameplayEffectSpec
    {
        //GE效果数据
        public GameplayEffectData GEData { get; }

        //来源
        public object Source { get; }

        //剩余持续时间
        public float RemainingDuration { get; set; }

        //剩余周期时间
        public float RemainingPeriod { get; set; }

        //堆叠层数
        public int StackCount { get; set; }

        public E_EffectStacking Stacking;
        //持续时间刷新策略
        public E_EffectDurationRefresh DurationRefresh;
        //周期重置策略
        public E_EffectPeriodReset PeriodReset;
        //到期策略
        public E_EffectExpiration Expiration;

        //已应用的修饰符列表
        public List<StatModifier> AppliedModifiers { get; } = new();


        // 构造函数
        public GameplayEffectSpec(GameplayEffectData definition, object source)
        {
            this.GEData = definition;
            this.Source = source;

            //如果是持续效果
            if (definition.Duration == E_EffectDuration.HasDuration)
            {
                //初始化持续时间
                RemainingDuration = definition.DurationValue;
            }
            //如果是永久效果
            else if (definition.Duration == E_EffectDuration.Infinite)
            {
                RemainingDuration = float.MaxValue;
            }

            //如果效果是周期执行
            if (definition.IsPeriodic)
            {
                //如果是应用时立即触发
                if (definition.ExecutePeriodicEffectOnApplication)
                {
                    RemainingPeriod = 0f;
                }
                else //不然就初始化周期时间
                {
                    RemainingPeriod = definition.Period;
                }
            }
            StackCount = 1;
        }

        //是否已经过期
        public bool IsExpired => GEData.Duration != E_EffectDuration.Infinite
                                                                && RemainingDuration <= 0f;

        //是否到了触发周期效果的时间了 (周期效果才需要判断)
        public bool ShouldExecutePeriod => GEData.IsPeriodic
                                           && RemainingPeriod <= 0f;
        /// <summary>
        /// 重置周期时间
        /// </summary>
        public void ResetPeriod()
        {
            if (GEData.IsPeriodic)
            {
                RemainingPeriod = GEData.Period;
            }
        }

        /// <summary>
        /// 刷新持续时间
        /// </summary>
        public void RefreshDuration()
        {
            if (GEData.HasDuration)
            {
                RemainingDuration = GEData.DurationValue;
            }
        }

        /// <summary>
        /// 创建修饰符列表
        /// </summary>
        /// <param name="source">来源</param>
        /// <returns>修饰符列表</returns>
        public List<StatModifier> CreateModifiers(object source)
        {
            List<StatModifier> result = new();
            foreach (var config in GEData.StatModifierConfig)
            {
                var modifier = new StatModifier(
                    System.Guid.NewGuid().ToString(),
                    config.type,
                    config.value,
                    source,
                    config.priority
                );
                result.Add(modifier);
            }
            return result;
        }
    }

}