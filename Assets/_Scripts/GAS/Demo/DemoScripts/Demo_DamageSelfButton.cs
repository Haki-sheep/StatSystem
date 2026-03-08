using GAS.Component;
using GAS.StateSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GAS.Demo
{
    /// <summary>
    /// 测试按钮 - 伤害玩家以测试治疗术
    /// </summary>
    public class Demo_DamageSelfButton : MonoBehaviour
    {
        [Header("玩家引用")]
        [SerializeField] private Demo_Player player;
        
        [Header("伤害值")]
        [SerializeField] private float damageAmount = 70f;
        
        [Header("伤害属性ID")]
        [SerializeField] private string hpStatId = "PlayerStats_HP";

        private void Start()
        {
            // 自动获取Player
            if (player == null)
            {
                player = FindObjectOfType<Demo_Player>();
            }
            
            // 绑定按钮点击
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (player == null)
            {
                Debug.LogWarning("[伤害按钮] 未找到玩家!");
                return;
            }

            var statController = player.GetStatController();
            if (statController == null)
            {
                Debug.LogWarning("[伤害按钮] 玩家没有StatController!");
                return;
            }

            // 查找HP属性
            var hpStat = statController.GetImStat(hpStatId);
            if (hpStat == null)
            {
                Debug.LogWarning($"[伤害按钮] 未找到HP属性! 现有属性: {string.Join(", ", statController.StatDict.Keys)}");
                
                // 尝试查找任意ImStat
                foreach(var kvp in statController.StatDict)
                {
                    if (kvp.Value is IImmediateStat im)
                    {
                        Debug.Log($"[伤害按钮] 找到ImStat: {kvp.Key}, 当前值: {im.CurrentValue}");
                        hpStat = im;
                        hpStatId = kvp.Key;
                        break;
                    }
                }
                
                if (hpStat == null) return;
            }

            Debug.Log($"[伤害按钮] 修改前HP: {hpStat.CurrentValue}");

            // 造成伤害
            statController.ChangeAttributeValue(hpStatId, -damageAmount, E_ModifierType.FlatAdd);
            
            float currentHP = statController.GetCurrentValue(hpStatId);
            Debug.Log($"[伤害按钮] 造成 {damageAmount} 点伤害，玩家剩余血量: {currentHP}");
        }
    }
}
