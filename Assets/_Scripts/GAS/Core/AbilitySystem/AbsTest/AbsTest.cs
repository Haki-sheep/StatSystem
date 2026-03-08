using GAS.StateSystem;
using UnityEngine;

namespace GAS.AbilitySystem
{
    public class AbsTest : MonoBehaviour
    {
        public GameplayAbility myAbility;
        public StatController statController;
        private AbilitySpec _abilitySpec;

        void Start()
        {
            // 1. 初始化属性控制器
            if (statController != null)
            {
                statController.Init();
            }

            // 2. 从技能创建技能实例
            if (myAbility != null)
            {
                _abilitySpec = myAbility.CreateAbilitySpec();
                Debug.Log($"技能实例创建成功: {myAbility.abilityName}");
            }
        }

        void Update()
        {
            // ⭐ 新增：更新冷却
            if (_abilitySpec?.cooldown != null)
            {
                _abilitySpec.cooldown.UpdateCooldown(true, Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TestAbility();
            }
        }

        void TestAbility()
        {
            if (_abilitySpec == null)
            {
                Debug.LogWarning("技能实例为空!");
                return;
            }

            // ⭐ 新增：详细调试日志
            var cooldown = _abilitySpec.cooldown;
            var cost = _abilitySpec.cost;

            Debug.Log($"=== 技能激活检查 ===");
            Debug.Log($"冷却: {cooldown?.IsOncooldown} (剩余 {cooldown?.RamingCooldown:F1}s)");

            if (cost != null)
            {
                float costValue = cost.CalculateCostValue(statController);
                var stat = statController?.GetImStat(cost.statId);
                Debug.Log($"消耗: {cost.statId} = {costValue} (当前: {stat?.CurrentValue})");
            }

            // 检查能否激活
            if (_abilitySpec.CanActivate(statController))
            {
                _abilitySpec.Activate(statController);
            }
        }
    }
}