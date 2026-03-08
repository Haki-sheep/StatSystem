
using GAS.StateSystem;

namespace GAS.AbilitySystem
{
    /// <summary>
    /// 技能效果类型
    /// </summary>
    public enum E_CostType
    {
        Fixed,           // 固定值
        PercentMax,       // 百分比最大属性值
        PercentCurrent    // 百分比当前属性值
    }

    /// <summary>
    /// 技能消耗类 
    /// 技能所影响到的就是加减数值 而非GE那种有点复杂的效果
    /// 如果真的需要百分比计算 只需要计算完毕然后加到属性上去即可 一样的
    /// </summary>
    public class AbilityCost
    {
        //消耗的属性ID
        public string statId;
        //消耗类型
        public E_CostType costType;
        //消耗值
        public float value;

        public AbilityCost(){
          
        }

        /// <summary>
        /// 计算消耗值
        /// </summary>
        /// <param name="statController"></param>
        /// <returns></returns>
        public float CalculateCostValue(StatController statController)
        {
            var stat = statController.GetImStat(statId);
            if (stat == null) return 0;

            return costType switch
            {
                E_CostType.Fixed => value,
                E_CostType.PercentMax => stat.MaxValue * value / 100,
                E_CostType.PercentCurrent => stat.CurrentValue * value / 100,
                _ => 0,
            };
        }


        /// <summary>
        /// 应用消耗
        /// </summary>
        /// <param name="statController"></param>
        /// <returns></returns>
        public bool Apply(StatController statController)
        {
            if(statController is null) return false;
            float cost = CalculateCostValue(statController);
            if (cost > 0)
            {
                statController.GetImStat(statId).ChangeValue(-cost, E_ModifierType.FlatAdd);
            }
            return true;
        }

        /// <summary>
        /// 检查是否足够支付
        /// </summary>
        public bool CanApply(StatController statController)
        {
            if(statController is null) return false;
            var stat = statController.GetImStat(statId);
            if (stat is null) return false;

            return stat.CurrentValue >= CalculateCostValue(statController);
        }
    }
}
