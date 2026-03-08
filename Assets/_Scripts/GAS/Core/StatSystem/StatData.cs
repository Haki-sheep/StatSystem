using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.StateSystem
{
    /// <summary>
    /// 属性类型
    /// </summary>
    public enum E_StatType
    {
        [LabelText("被动属性 (Stat)")]
        Passive,    // 攻击力、防御力等长期持有的属性，使用 FinalValue
        
        [LabelText("即时属性 (ImStat)")]
        Immediate,  // HP、MP 等直接变化的资源属性，使用 CurrentValue
    }

    [CreateAssetMenu(fileName = "StatData", menuName = "GAS/StatDefinition", order = 0)]
    public class StatData : SerializedScriptableObject
    {
        [Header("属性类型")]
        [SerializeField] private E_StatType statType = E_StatType.Passive;

        [Header("基础值")]
        [SerializeField] private float baseValue = 100f;

        [Header("限制范围")]
        [SerializeField, LabelText("最小值")] private float minValue = float.MinValue;
        [SerializeField, LabelText("最大值")] private float maxValue = float.MaxValue;

        [Header("运行时设置")]
        [SerializeField] private bool resetCurrentValueOnPlay = true;

        public E_StatType StatType => statType;
        public float BaseValue => baseValue;
        public float MinValue => minValue;
        public float MaxValue => maxValue;
        public bool ResetCurrentValueOnPlay => resetCurrentValueOnPlay;
    }
}
