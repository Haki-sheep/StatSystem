using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Core.GameplayEffect
{
    /// <summary>
    /// 时间策略
    /// </summary>
    public enum E_EffectDuration
    {
        [LabelText("即时")] Instant = 0,      // 即时（应用一次就结束）
        [LabelText("持续")] HasDuration = 1,  // 有持续时间（持续一段时间后消失）
        [LabelText("永久")] Infinite = 2      // 永久（除非被移除）
    }

    /// <summary>
    /// 叠加策略
    /// </summary>
    public enum E_EffectStacking
    {
        [LabelText("不允许堆叠")] None = 0,              // 不允许堆叠
        [LabelText("层数+1")] StackUpper = 1,           // 层数+1
        [LabelText("按来源覆盖")] OverrideBySource = 2, // 按来源覆盖
        [LabelText("设置为最高层数")] SetToMaxStacks = 3   // 设置为最高层数
    }

    /// <summary>
    /// 叠加持续时间刷新策略
    /// </summary>
    public enum E_EffectDurationRefresh
    {
        [LabelText("不刷新")] NeverRefresh = 0,                    // 叠加时不刷新持续时间
        [LabelText("应用成功时刷新")] RefreshOnSuccessfulApplication = 1  // 叠加时刷新持续时间
    }
    /// <summary>
    /// 周期重置策略
    /// </summary>
    public enum E_EffectPeriodReset
    {
        [LabelText("不重置")] NeverReset = 0, //应用新的GE以后不重置周期
        [LabelText("应用成功时重置")] ResetOnSuccessfulApplication = 1 //应用成功时重置周期
    }

    /// <summary>
    /// 到期策略
    /// </summary>
    public enum E_EffectExpiration
    {
        [LabelText("清除整个堆栈")] ClearEntireStack = 0,
        [LabelText("移除单层并刷新")] RemoveSingleStackAndRefreshDuration = 1,
        [LabelText("只刷新持续时间")] RefreshDuration = 2
    }
}