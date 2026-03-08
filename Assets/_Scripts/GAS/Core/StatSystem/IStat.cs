using System;

namespace GAS.StateSystem
{
    /// <summary>
    /// 属性接口
    /// </summary>
    public interface IStat
    { 
        /// <summary>
        /// 基础值
        /// </summary>
        float BaseValue { get; }

        /// <summary>
        /// 所属控制器
        /// </summary>
        StatController Controller { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 强制重算
        /// </summary>
        void ForceRecalculate();
    }

    /// <summary>
    /// 被动属性（Stat）
    /// 攻击力、防御力、速度等长期持有的属性
    /// </summary>
    public interface IPassiveStat : IStat
    {
        /// <summary>
        /// 最终值
        /// </summary>
        float FinalValue { get; }

        /// <summary>
        /// 添加修饰符
        /// </summary>
        void AddModifier(StatModifier modifier);

        /// <summary>
        /// 移除指定Id的修饰符
        /// </summary>
        bool RemoveModifier(string modifierId);

        /// <summary>
        /// 移除指定修饰符
        /// </summary>
        void RemoveModifier(StatModifier modifier);

        /// <summary>
        /// 移除所有来源的修饰符
        /// </summary>
        void RemoveModifiersFromSource(object source);

        /// <summary>
        /// 清空所有修饰符
        /// </summary>
        void ClearModifiers();

        /// <summary>
        /// 值变化事件
        /// </summary>
        event Action OnValueChanged;
    }

    /// <summary>
    /// 即时属性（ImStat）
    /// HP、MP 等直接变化的资源属性
    /// </summary>
    public interface IImmediateStat : IStat
    {
        /// <summary>
        /// 当前值
        /// </summary>
        float CurrentValue { get; }

        /// <summary>
        /// 最大值（资源上限）
        /// </summary>
        float MaxValue { get; }

        /// <summary>
        /// 瞬时变化（直接修改当前值）
        /// </summary>
        void ChangeValue(float magnitude, E_ModifierType modifierType);

        /// <summary>
        /// 恢复到终值
        /// </summary>
        void Restore();

        /// <summary>
        /// 当前值变化事件
        /// </summary>
        event Action CurValueChanged;
    }
}
