using System.Collections.Generic;
using GAS.Component;
using UnityEngine;

namespace GAS.Targeting
{
    /// <summary>
    /// 目标选择系统 - 根据 TargetType 获取目标
    /// </summary>
    public class TargetingSystem
    {
        /// <summary>
        /// 获取目标数据
        /// </summary>
        public static TargetData GetTargetData(
            AbilitySystemComponent caster,
            TargetType targetType,
            Vector3 origin,
            Vector3 direction,
            float radius = 5f,
            float angle = 60f,
            Vector3? boxSize = null,
            LayerMask layerMask = default)
        {
            var targetData = new TargetData
            {
                TargetType = targetType,
                TargetLocation = origin + direction * radius
            };

            bool? isEnemy = targetType switch
            {
                TargetType.SingleEnemy or TargetType.AreaEnemy or TargetType.ConeEnemy or TargetType.BoxEnemy => true,
                TargetType.SingleAlly or TargetType.AreaAlly or TargetType.ConeAlly or TargetType.BoxAlly => false,
                _ => null
            };  

            //根据不同的目标类型得到目标列表
            switch (targetType)
            {
                case TargetType.None:
                    break;

                case TargetType.Self:
                    targetData.TargetList.Add(caster);
                    break;

                case TargetType.SingleEnemy:
                case TargetType.SingleAlly:
                    var single = GetSingleTarget(caster, origin, direction, radius, isEnemy);
                    if (single != null)
                        targetData.TargetList.Add(single);
                    break;

                case TargetType.AreaEnemy:
                case TargetType.AreaAlly:
                case TargetType.AreaAll:
                    targetData.TargetList.AddRange(GetSphereTargets(caster, origin, radius, isEnemy));
                    break;

                case TargetType.ConeEnemy:
                case TargetType.ConeAlly:
                case TargetType.ConeAll:
                    targetData.TargetList.AddRange(GetConeTargets(caster, origin, direction, radius, angle, isEnemy));
                    break;

                case TargetType.BoxEnemy:
                case TargetType.BoxAlly:
                case TargetType.BoxAll:
                    var size = boxSize ?? new Vector3(radius * 2, 2, radius);
                    targetData.TargetList.AddRange(GetBoxTargets(caster, origin, direction, size, isEnemy));
                    break;

                case TargetType.Cursor:
                    targetData.TargetLocation = origin;
                    break;
            }

            return targetData;
        }

        #region 基础检测

        /// <summary>
        /// 获取球形范围内的目标
        /// </summary>
        public static List<AbilitySystemComponent> GetSphereTargets(
            AbilitySystemComponent caster,
            Vector3 center,
            float radius,
            bool? isEnemy)
        {
            var targets = new List<AbilitySystemComponent>();
            int maxHits = 20;
            var buffer = new Collider[maxHits];
            int count = Physics.OverlapSphereNonAlloc(center, radius, buffer);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent(out AbilitySystemComponent asc) && IsValidTarget(caster, asc, isEnemy))
                {
                    targets.Add(asc);
                }
            }

            return targets;
        }

        /// <summary>
        /// 获取扇形范围内的目标
        /// </summary>
        public static List<AbilitySystemComponent> GetConeTargets(
            AbilitySystemComponent caster,
            Vector3 origin,
            Vector3 direction,
            float radius,
            float angle,
            bool? isEnemy)
        {
            var targets = new List<AbilitySystemComponent>();
            
            int maxHits = 20;
            var buffer = new Collider[maxHits];
            int count = Physics.OverlapSphereNonAlloc(origin, radius, buffer);

            for (int i = 0; i < count; i++)
            {
                if (!buffer[i].TryGetComponent(out AbilitySystemComponent asc))
                    continue;

                Vector3 toTarget = (buffer[i].transform.position - origin).normalized;
                float angleToTarget = Vector3.Angle(direction, toTarget);

                if (angleToTarget <= angle / 2 && IsValidTarget(caster, asc, isEnemy))
                {
                    targets.Add(asc);
                }
            }

            return targets;
        }

        /// <summary>
        /// 获取矩形(盒子)范围内的目标
        /// </summary>
        public static List<AbilitySystemComponent> GetBoxTargets(
            AbilitySystemComponent caster,
            Vector3 origin,
            Vector3 direction,
            Vector3 size,
            bool? isEnemy)
        {
            var targets = new List<AbilitySystemComponent>();
            
            int maxHits = 20;
            var buffer = new Collider[maxHits];
            
            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 center = origin + direction * size.z / 2;
            int count = Physics.OverlapBoxNonAlloc(center, size / 2, buffer, rotation);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent(out AbilitySystemComponent asc) && IsValidTarget(caster, asc, isEnemy))
                {
                    targets.Add(asc);
                }
            }

            return targets;
        }

        /// <summary>
        /// 获取单体目标（最近的）
        /// </summary>
        private static AbilitySystemComponent GetSingleTarget(
            AbilitySystemComponent caster,
            Vector3 origin,
            Vector3 direction,
            float radius,
            bool? isEnemy)
        {
            // 使用 NonAlloc 版本
            int maxHits = 50;
            var buffer = new Collider[maxHits];
            int count = Physics.OverlapSphereNonAlloc(origin, radius, buffer);
            
            AbilitySystemComponent bestTarget = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!buffer[i].TryGetComponent(out AbilitySystemComponent target))
                    continue;

                if (!IsValidTarget(caster, target, isEnemy))
                    continue;

                float distance = Vector3.Distance(origin, buffer[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }

        #endregion

        #region 验证

        /// <summary>
        /// 验证目标是否有效
        /// </summary>
        public static bool IsValidTarget(AbilitySystemComponent caster, AbilitySystemComponent target, bool? isEnemy)
        {
            if (caster == null || target == null || caster == target)
                return false;

            // TODO: 需要根据阵营系统或 GameplayTag 判断敌我
            // 目前简单返回 true，实际需要结合 GameplayTagContainer 判断
            return true;
        }

        #endregion
    }
}