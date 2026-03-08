namespace GAS.AbilitySystem
{
    /// <summary>
    /// 技能冷却
    /// 本质上是一个计时器
    /// </summary>
    public class AbilityCooldown
    {
        //冷却时间
        public float cooldownTime;

        //剩余冷却 时间 = CD时间
        private float remaingCooldown;

        //是否正在冷却中
        public bool IsOncooldown => remaingCooldown>0;

        //剩余冷却 时间
        public float RamingCooldown =>remaingCooldown;


        /// <summary>
        /// 进入CD
        /// </summary>
        /// <returns></returns>
        public void StartCooldown(){
            remaingCooldown = cooldownTime;
        }

        /// <summary>
        /// 能否释放技能
        /// </summary>
        /// <returns></returns>
        public bool CanUseAbility(){
            return !IsOncooldown;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public void UpdateCooldown(bool needUpdate,float daltaTime){
            if(needUpdate is false) return;
            if(remaingCooldown >0){
                remaingCooldown -= daltaTime;
                if(remaingCooldown <0){
                    remaingCooldown = 0;
                }
            }
        }


        /// <summary>
        /// 直接减少CD
        /// </summary>
        /// <param name="reduceValue"></param>
        public void ReduceCooldown(float reduceValue){
            remaingCooldown -= reduceValue;
            if(remaingCooldown <0){
                remaingCooldown = 0;
            }
        }

    }
}
