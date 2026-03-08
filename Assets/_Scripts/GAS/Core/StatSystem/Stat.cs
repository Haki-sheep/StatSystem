  using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GAS.StateSystem
{
    /// <summary>
    /// 被动属性
    /// 攻击力、防御力、速度等长期持有的属性
    /// </summary>
    [System.Serializable]
    public class Stat : IPassiveStat
    {
        //控制器
        private readonly StatController controller;
        public StatController Controller => controller;

        //原值
        private readonly float baseValue;
        public float BaseValue => baseValue;

        //最小/最大值
        private readonly float minValue;
        private readonly float maxValue;
        
        //脏标志：是否需要重新计算
        private bool m_IsDirty = true;

        //最终值
        private float finalValue;
        public float FinalValue
        {
            get
            {
                if (m_IsDirty)
                {
                    CalculateFinalValue();
                }
                return finalValue;
            }
        }

        //修改事件
        public event Action OnValueChanged;
        
        private readonly List<StatModifier> modifiers = new List<StatModifier>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Stat(StatData definition, StatController controller)
        {
            this.controller = controller;
            this.baseValue = definition.BaseValue;
            this.minValue = definition.MinValue;
            this.maxValue = definition.MaxValue;
            this.finalValue = baseValue;
        }

        /// <summary>
        /// 子类构造函数需调用 base(definition, controller)
        /// </summary>
        protected Stat() { }

        public virtual void Initialize()
        {
            CalculateFinalValue(); // 立即计算一次最终值
        }

        /// <summary>
        /// 添加修饰符
        /// </summary>
        public virtual void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            m_IsDirty = true;
        }

        /// <summary>
        /// 移除指定Id的修饰符
        /// </summary>
        public virtual bool RemoveModifier(string modifierId)
        {
            int removedCount = modifiers.RemoveAll(m => m.Id == modifierId);
            if (removedCount > 0)
            {
                m_IsDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定修饰符
        /// </summary>
        public virtual void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
            m_IsDirty = true;
        }

        /// <summary>
        /// 移除所有来源的修饰符
        /// </summary>
        public virtual void RemoveModifiersFromSource(object source)
        {
            int count = modifiers.RemoveAll(m => m.Source == source);
            if (count > 0) m_IsDirty = true;
        }

        /// <summary>
        /// 清空所有修饰符
        /// </summary>
        public virtual void ClearModifiers()
        {
            modifiers.Clear();
            m_IsDirty = true;
        }

        /// <summary>
        /// 计算最终值（4阶段计算）
        /// </summary>
        protected virtual void CalculateFinalValue()
        {
            // 0. 升序排列：先按类型，再按优先级
            var sortedModifiers = modifiers
                .OrderBy(m => m.ModifierType)
                .ThenBy(m => m.Priority)
                .ToList();

            // 1. 基础值
            float result = baseValue;

            // 2.1 加减（FlatAdd）
            float flatAdd = sortedModifiers
                .Where(m => m.ModifierType == E_ModifierType.FlatAdd)
                .Sum(m => m.Value);
            result += flatAdd;

            // 2.2 百分比（PercentageAdd）
            float percentageAdd = sortedModifiers
                .Where(m => m.ModifierType == E_ModifierType.PercentageAdd)
                .Sum(m => m.Value);
            result *= (1 + percentageAdd / 100f); // 1 + 50/100 = 1.5

            // 2.3 最终加减（FinalAdd）
            float finalAdd = sortedModifiers
                .Where(m => m.ModifierType == E_ModifierType.FinalAdd)
                .Sum(m => m.Value);
            result += finalAdd;

            // 2.4 最终百分比（FinalPercentage）
            float finalPercentage = sortedModifiers
                .Where(m => m.ModifierType == E_ModifierType.FinalPercentage)
                .Sum(m => m.Value);
            result *= (1 + finalPercentage / 100f);

            // 3. 夹逼（Clamp）
            result = Mathf.Clamp(result, minValue, maxValue);

            // 4. 缓存结果
            finalValue = result;
            m_IsDirty = false;

            // 触发事件
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// 强制重算
        /// </summary>
        public virtual void ForceRecalculate()
        {
            m_IsDirty = true;
            _ = FinalValue; // 触发计算
        }
    }
}
