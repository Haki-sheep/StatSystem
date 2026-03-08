using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.StateSystem
{
    /// <summary>
    /// 属性控制器 
    /// </summary> 
    public class StatController : MonoBehaviour
    {
        [SerializeField] private List<StatData> statDataList = new();

        private readonly Dictionary<string, IStat> statDict = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, IStat> StatDict => statDict;

        public bool IsInit { get; private set; }
        
        public void Init()
        {
            if (IsInit) return;

            statDict.Clear();

            foreach (var data in statDataList)
            {
                if (data == null) continue;

                string key = data.name;

                // 根据属性类型创建对应的实例
                IStat stat = data.StatType == E_StatType.Immediate 
                    ? new ImStat(data, this) 
                    : new Stat(data, this);
                stat.Initialize();

                statDict[key] = stat;
            }
            IsInit = true;
        }


        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        public IStat GetStat(string statName)
        {
            if (string.IsNullOrWhiteSpace(statName)) return null;
            statDict.TryGetValue(statName, out var stat);
            return stat;
        }

        /// <summary>
        /// 获取被动属性
        /// </summary>
        public IPassiveStat GetPassiveStat(string statName)
        {
            return GetStat(statName) as IPassiveStat;
        }

        /// <summary>
        /// 获取即时属性
        /// </summary>
        public IImmediateStat GetImStat(string statName)
        {
            return GetStat(statName) as IImmediateStat;
        }

        /// <summary>
        /// 获取属性基础值
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        public float GetValue(string statName)
        {
            var stat = GetStat(statName);
            return stat?.BaseValue ?? 0f;
        }

        /// <summary>
        /// 获取属性当前值
        /// ImStat: 返回 CurrentValue
        /// Stat: 返回 FinalValue
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        public float GetCurrentValue(string statName)
        {
            var imStat = GetImStat(statName);
            if (imStat != null)
            {
                // ImStat 使用 CurrentValue
                return imStat.CurrentValue;
            }
            
            // Stat 使用 FinalValue
            var passiveStat = GetPassiveStat(statName);
            return passiveStat?.FinalValue ?? 0f;
        }

        /// <summary>
        /// 添加修饰符
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="modifier"></param>
        public void AddModifier(string statName, StatModifier modifier)
        {
            var passiveStat = GetPassiveStat(statName);
            if (passiveStat == null || modifier == null) return;
            passiveStat.AddModifier(modifier);
        }

        /// <summary>
        /// 从来源移除修饰符
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="source"></param>
        public void RemoveModifiersFromSource(string statName, object source)
        {
            var passiveStat = GetPassiveStat(statName);
            if (passiveStat is null) return;
            passiveStat.RemoveModifiersFromSource(source);
        }

        /// <summary>
        /// 对 ImStat 做瞬时变化
        /// </summary>
        public void ChangeAttributeValue(string statName, float magnitude, E_ModifierType modifierType, object source = null)
        {
            var imStat = GetImStat(statName);
            if (imStat is null) return;

            imStat.ChangeValue(magnitude, modifierType);
        }
    }
}
