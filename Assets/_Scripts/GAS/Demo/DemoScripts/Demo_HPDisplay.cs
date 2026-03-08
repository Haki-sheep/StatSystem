using GAS.Component;
using GAS.StateSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GAS.Demo
{
    /// <summary>
    /// 实时显示玩家和敌人血量的UI管理器
    /// </summary>
    public class Demo_HPDisplay : MonoBehaviour
    {
        [Header("玩家引用")]
        [SerializeField] private Demo_Player player;
        
        [Header("敌人引用")]
        [SerializeField] private Transform enemyTarget;
        
        [Header("UI文本")]
        [SerializeField] private Text playerHPText;
        [SerializeField] private Text enemyHPText;

        private void Start()
        {
            // 尝试自动获取Player
            if (player == null)
            {
                player = FindObjectOfType<Demo_Player>();
            }
            
            // 初始化玩家的StatController
            if (player != null)
            {
                var playerSC = player.GetStatController();
                if (playerSC != null && !playerSC.IsInit)
                {
                    playerSC.Init();
                }
            }
            
            // 初始化敌人的StatController
            if (enemyTarget != null)
            {
                var enemySC = enemyTarget.GetComponent<StatController>();
                if (enemySC != null && !enemySC.IsInit)
                {
                    enemySC.Init();
                }
            }
        }

        private void Update()
        {
            UpdatePlayerHP();
            UpdateEnemyHP();
        }

        private void UpdatePlayerHP()
        {
            if (player == null || playerHPText == null) return;

            var statController = player.GetStatController();
            if (statController == null) 
            {
                playerHPText.text = "玩家血量: (无StatController)";
                return;
            }

            // 查找ImStat
            IImmediateStat hpStat = FindFirstImStat(statController);
            if (hpStat == null)
            {
                playerHPText.text = "玩家血量: (无HP属性)";
                Debug.LogWarning($"[HPDisplay] 玩家有 {statController.StatDict.Count} 个属性，但没有ImStat，Keys: {string.Join(", ", statController.StatDict.Keys)}");
                return;
            }

            Debug.Log($"[HPDisplay] 玩家HP: Current={hpStat.CurrentValue}, Max={hpStat.MaxValue}");

            float currentHP = hpStat.CurrentValue;
            float maxHP = hpStat.MaxValue;
            
            // 如果最大值异常，使用基础值
            if (maxHP > 100000f)
            {
                maxHP = hpStat.BaseValue;
            }

            playerHPText.text = $"玩家血量: {currentHP:F0} / {maxHP:F0}";
            
            // 血量低时变红
            if (maxHP > 0 && currentHP / maxHP < 0.3f)
                playerHPText.color = Color.red;
            else
                playerHPText.color = Color.white;
        }

        private void UpdateEnemyHP()
        {
            if (enemyTarget == null || enemyHPText == null) 
            {
                if(enemyHPText != null)
                    enemyHPText.text = "敌人血量: --";
                return;
            }

            var statController = enemyTarget.GetComponent<StatController>();
            if (statController == null) 
            {
                enemyHPText.text = "敌人血量: (无StatController)";
                Debug.LogWarning("[HPDisplay] 敌人没有StatController组件!");
                return;
            }

            // 打印调试信息
            if (statController.StatDict.Count == 0)
            {
                enemyHPText.text = "敌人血量: (Stat未Init)";
                Debug.LogWarning($"[HPDisplay] 敌人StatController未初始化，属性数量: {statController.StatDict.Count}");
                return;
            }

            // 查找ImStat
            IImmediateStat hpStat = FindFirstImStat(statController);
            if (hpStat == null)
            {
                enemyHPText.text = "敌人血量: (无HP属性)";
                Debug.LogWarning($"[HPDisplay] 敌人有 {statController.StatDict.Count} 个属性，但没有ImStat");
                return;
            }

            float currentHP = hpStat.CurrentValue;
            float maxHP = hpStat.MaxValue;
            
            // 如果最大值异常，使用基础值
            if (maxHP > 100000f)
            {
                maxHP = hpStat.BaseValue;
            }

            enemyHPText.text = $"敌人血量: {currentHP:F0} / {maxHP:F0}";
            
            // 血量低时变红
            if (maxHP > 0 && currentHP / maxHP < 0.3f)
                enemyHPText.color = Color.red;
            else
                enemyHPText.color = Color.white;
        }

        private IImmediateStat FindFirstImStat(StatController sc)
        {
            foreach (var kvp in sc.StatDict)
            {
                if (kvp.Value is IImmediateStat imStat)
                {
                    return imStat;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置敌人目标
        /// </summary>
        public void SetEnemyTarget(Transform enemy)
        {
            enemyTarget = enemy;
        }
    }
}
