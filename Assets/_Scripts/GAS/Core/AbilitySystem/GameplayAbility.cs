using GAS.StateSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.AbilitySystem
{
    /// <summary>
    /// 游戏技能基类
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewAbility", menuName = "GAS/GameplayAbility")]
    public  class GameplayAbility : ScriptableObject 
    {
        [LabelText("技能图标")]public Sprite icon;

        [LabelText("技能名称")]public string abilityName;

        [TextArea]
        [LabelText("技能描述")]public string description;
        [LabelText("冷却时间")]public float cooldownTime = 1f;

        [Header("消耗")]
        [LabelText("消耗属性ID")] public string costStatId;
        [LabelText("消耗类型")]public E_CostType costType;
        [LabelText("消耗数值")]public float costValue;


        /// <summary>
        /// 激活技能
        /// </summary>
        public virtual void Activate(StatController statController){
            Debug.Log($"激活技能: {abilityName}");
        }

        /// <summary>
        /// 中断技能
        /// </summary>
        public virtual void InterruptTask()
        {
            Debug.Log($"中断技能: {abilityName}");
        }


        public AbilitySpec CreateAbilitySpec(){
            
            AbilitySpec abilitySpec = new AbilitySpec(){
                ability = this,
                cooldown = new AbilityCooldown { cooldownTime = this.cooldownTime },
                cost = new AbilityCost(){
                    statId = this.costStatId,
                    costType = this.costType,
                    value = this.costValue
                }
            };
            return abilitySpec;
        }
    }
}
