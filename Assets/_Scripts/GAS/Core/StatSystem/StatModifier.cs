namespace GAS.StateSystem
{
    /// <summary>
    /// 修饰符类型（4阶段计算）
    /// </summary>
    public enum E_ModifierType
    {
        FlatAdd = 0,           // 加减（第一阶段）
        PercentageAdd = 1,     // 百分比（第二阶段）
        FinalAdd = 2,          // 最终加减（第三阶段）
        FinalPercentage = 3    // 最终百分比（第四阶段）
    }

    /// <summary>
    /// 修饰符基类
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        public string Id; // 修饰符Id
        public object Source; // 来源
        public float Value; // 修饰值
        public int Priority;// 排序优先级
        public E_ModifierType ModifierType; // 修饰符类型

        public StatModifier()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StatModifier(string id, E_ModifierType type, float value, object source = null, int priority = 0)
        {
            Id = id;
            ModifierType = type;
            Value = value;
            Source = source;
            Priority = priority;
        }
    }

    /// <summary>
    /// 泛型修饰符（避免装箱）
    /// </summary>
    [System.Serializable]
    public class StatModifier<T> : StatModifier
    {
        public new T Source { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StatModifier(T source, E_ModifierType type, float value, int priority = 0)
        {
            Id = System.Guid.NewGuid().ToString();
            ModifierType = type;
            Value = value;
            this.Source = source;
            Priority = priority;
        }

        /// <summary>
        /// 构造函数（带Id）
        /// </summary>
        public StatModifier(string id, T source, E_ModifierType type, float value, int priority = 0)
        {
            Id = id;
            ModifierType = type;
            Value = value;
            this.Source = source;
            Priority = priority;
        }

        public override string ToString() => Value.ToString();
    }
}
