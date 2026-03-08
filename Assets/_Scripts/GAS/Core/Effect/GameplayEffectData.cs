using System;
using System.Collections.Generic;
using GAS.StateSystem;
using GAS.TagSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Core.GameplayEffect
{
    /// <summary>
    /// GameplayEffect 数据蓝图 (ScriptableObject)
    /// 定义一个Buff/Debuff的所有配置
    /// </summary> 
    [CreateAssetMenu(fileName = "GE_Data", menuName = "GAS/GameplayEffectData")]
    public class GameplayEffectData : SerializedScriptableObject
    {
        [Header("基础信息")]

        [SerializeField, LabelText("分类标签")]
        private string[] gameplayTags = Array.Empty<string>();
        
        [SerializeField, LabelText("需要标签")]
        private string[] requiredNeedTags = Array.Empty<string>();
        
        [SerializeField, LabelText("禁止标签")]
        private string[] requiredBanTags = Array.Empty<string>();

        [Header("时间")] // ------

        [SerializeField, LabelText("时间策略")]
        private E_EffectDuration durationPolicy = E_EffectDuration.Instant;

        [SerializeField, LabelText("持续时间")] private float duration = 5f;

        [Header("触发周期")]
        [SerializeField, LabelText("是否支持周期效果")] private bool isPeriodic = false;

        [ShowIf("isPeriodic")]
        [SerializeField, LabelText("周期(Tick)")] private float period = 1f;

        [ShowIf("isPeriodic")]
        [SerializeField, LabelText("应用时立即触发")]
        private bool executePeriodicEffectOnApplication = true;

        [Header("堆叠")] // -----
        [SerializeField, LabelText("堆叠来源")]
        private E_EffectStacking stackingPolicy = E_EffectStacking.None;
        [SerializeField, LabelText("最大堆叠层数")] private int stackLimit = 1;
        [SerializeField, LabelText("持续时间刷新策略")] private E_EffectDurationRefresh durationRefreshPolicy = E_EffectDurationRefresh.RefreshOnSuccessfulApplication;
        [SerializeField, LabelText("周期重置策略")] private E_EffectPeriodReset periodResetPolicy = E_EffectPeriodReset.ResetOnSuccessfulApplication;
        [SerializeField, LabelText("到期策略")] private E_EffectExpiration expirationPolicy = E_EffectExpiration.ClearEntireStack;

        [Header("效果")]
        [SerializeField, LabelText("属性修饰符列表")] private List<StatModifierConfig> statModifierConfig = new();

        //对应属性
        public string[] GameplayTags => gameplayTags;
        public string[] RequiredNeedTags => requiredNeedTags;
        public string[] RequiredBanTags => requiredBanTags;
        public E_EffectDuration Duration => durationPolicy;
        public E_EffectDurationRefresh DurationRefresh => durationRefreshPolicy;
        public E_EffectPeriodReset PeriodReset => periodResetPolicy;
        public E_EffectExpiration Expiration => expirationPolicy;
        public float DurationValue => duration;//持续时间
        public float Period => period;
        public bool IsPeriodic => isPeriodic;
        public bool ExecutePeriodicEffectOnApplication => executePeriodicEffectOnApplication;
        public E_EffectStacking StackingPolicy => stackingPolicy;
        public int StackLimit => stackLimit;
        public List<StatModifierConfig> StatModifierConfig => statModifierConfig;

        /// 是否是即时效果
        public bool IsInstant => durationPolicy == E_EffectDuration.Instant;

        /// 是否有持续时间
        public bool HasDuration => durationPolicy == E_EffectDuration.HasDuration;

        /// 是否永久
        public bool IsInfinite => durationPolicy == E_EffectDuration.Infinite;

        /// 是否周期执行（Dot）
        public bool HasPeriod => isPeriodic;

    }

    /// <summary>
    /// 这里是修饰符的再包了一层用作配置 
    /// 为了和StatModifier这个运行实例区分开来
    /// 因为Config只关心改什么属性  StatModifier只关心谁给的、给多少
    /// </summary>
    [Serializable]
    public class StatModifierConfig
    {
        public string statId;// 属性ID
        public E_ModifierType type;// 加成类型
        public float value; // 值
        public int priority = 0; //优先级
    }
}