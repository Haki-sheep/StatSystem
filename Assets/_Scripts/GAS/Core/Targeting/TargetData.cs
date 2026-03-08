using System.Collections.Generic;
using GAS.Component;
using UnityEngine;

namespace GAS.Targeting
{
    /// <summary>
    /// 目标数据 - 包含技能目标的信息
    /// </summary>
    public class TargetData
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// 目标对象列表
        /// </summary>
        public List<AbilitySystemComponent> TargetList { get; set; } = new();

        /// <summary>
        /// 目标位置（用于 Cursor, Area 等）
        /// </summary>
        public Vector3? TargetLocation { get; set; }

        /// <summary>
        /// 是否有效
        /// 当目标类型不为None 且 目标列表不为空 或 目标位置不为空时 为有效
        /// </summary>
        public bool IsValid => TargetType != TargetType.None && (TargetList.Count > 0 || TargetLocation.HasValue);

        /// <summary>
        /// 单一目标便捷访问
        /// 当目标列表不为空时 返回目标列表的第一个元素
        /// 否则返回null
        /// </summary>
        public AbilitySystemComponent SingleTarget => TargetList.Count > 0 ? TargetList[0] : null;

    }
}