using UnityEngine;
using GAS.StateSystem;

public class Demo_GASControl : MonoBehaviour
{
    [Header("玩家引用")]
    [SerializeField] private Demo_Player player;

    [Header("技能ID")]
    [SerializeField] private string lightningBoltAbilityID = "LightningBolt";
    [SerializeField] private string healSelfAbilityID = "HealSelf";
    [SerializeField] private string speedBoostAbilityID = "SpeedBoost";
    [SerializeField] private string blizzardAbilityID = "Blizzard";

    private void Awake()
    {
        // 自动获取Player
        if (player == null)
        {
            player = FindObjectOfType<Demo_Player>();
            if (player == null)
            {
                Debug.LogError("Demo_GASControl: 未在场景中找到Demo_Player!");
            }
            else
            {
                Debug.Log($"Demo_GASControl: 已自动找到Player - {player.name}");
            }
        }
    }

    private void Update()
    {
        // 1键 - 雷电球
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[输入] 按下 1键 - 雷电球");
            TryActivateAbility(lightningBoltAbilityID);
        }

        // 2键 - 治疗自身
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[输入] 按下 2键 - 治疗自身");
            TryActivateAbility(healSelfAbilityID);
        }

        // 3键 - 加速自身
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("[输入] 按下 3键 - 加速自身");
            TryActivateAbility(speedBoostAbilityID);
        }

        // 4键 - 暴风雪
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("[输入] 按下 4键 - 暴风雪");
            TryActivateAbility(blizzardAbilityID);
        }

        // 5键 - 伤害自身
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("[输入] 按下 5键 - 伤害自身");
            DamageSelf();
        }
    }

    private void TryActivateAbility(string abilityID)
    {
        if (player == null)
        {
            Debug.LogWarning("Demo_GASControl: Player is null! 请确保场景中有Demo_Player对象");
            return;
        }

        Debug.Log($"Demo_GASControl: 尝试激活技能 - {abilityID}");
        bool success = player.TryActivateAbility(abilityID);
        if (success)
        {
            Debug.Log($"[输入] ✓ 成功激活技能: {abilityID}");
        }
        else
        {
            Debug.LogWarning($"[输入] ✗ 激活技能失败: {abilityID}");
        }
    }

    private void DamageSelf()
    {
        if (player == null)
        {
            Debug.LogWarning("Demo_GASControl: Player is null!");
            return;
        }

        var statController = player.GetStatController();
        if (statController == null)
        {
            Debug.LogWarning("[伤害自身] 玩家没有StatController!");
            return;
        }

        // 直接修改HP值
        float damage = 70f;
        var hpStat = statController.GetImStat("PlayerStats_HP");
        if (hpStat != null)
        {
            Debug.Log($"[伤害自身] 修改前HP: {hpStat.CurrentValue}");
            hpStat.ChangeValue(-damage, E_ModifierType.FlatAdd);
            Debug.Log($"[伤害自身] 造成 {damage} 点伤害，剩余HP: {hpStat.CurrentValue}");
        }
        else
        {
            Debug.LogWarning($"[伤害自身] 未找到PlayerStats_HP! 现有属性: {string.Join(", ", statController.StatDict.Keys)}");
        }
    }
}
