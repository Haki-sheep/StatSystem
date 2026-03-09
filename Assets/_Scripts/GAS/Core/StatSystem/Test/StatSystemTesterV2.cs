using System.Collections.Generic;
using GAS.StateSystem;
using UnityEngine;
using UnityEngine.UI;

public class StatSystemTesterV2 : MonoBehaviour
{
    [Header("UI 显示")]
    [SerializeField] private Text imStatText;    // 即时属性显示
    [SerializeField] private Text passiveStatText; // 被动属性显示

    [Header("属性数据")]
    [SerializeField] private List<StatData> statDataList;

    private StatController statController;

    // 用于存储测试生成的修饰符ID，便于移除
    private readonly List<string> _addedModifierIds = new();

    private void Awake()
    {
        statController = gameObject.AddComponent<StatController>();
        SetStatDataList(statDataList);
        statController.Init();
    }

    private void SetStatDataList(List<StatData> dataList)
    {
        var field = typeof(StatController).GetField("statDataList",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(statController, new List<StatData>(dataList));
    }

    private void Start()
    {
        Debug.Log("=== StatSystemV2 测试开始 ===");
        UpdateUI();
    }

    private void Update()
    {
        // 即时属性测试（数字键 1-4）
        if (Input.GetKeyDown(KeyCode.Alpha1)) TestImStatFlatAdd();
        if (Input.GetKeyDown(KeyCode.Alpha2)) TestImStatPercentageAdd();
        if (Input.GetKeyDown(KeyCode.Alpha3)) TestImStatFinalAdd();
        if (Input.GetKeyDown(KeyCode.Alpha4)) TestImStatHeal();  // 恢复满血

        // 被动属性测试（字母键 QWER）
        if (Input.GetKeyDown(KeyCode.Q)) TestPassiveFlatAdd();
        if (Input.GetKeyDown(KeyCode.W)) TestPassivePercentageAdd();
        if (Input.GetKeyDown(KeyCode.E)) TestPassiveFinalAdd();
        if (Input.GetKeyDown(KeyCode.R)) TestPassiveFinalPercentage();

        // 组合测试
        if (Input.GetKeyDown(KeyCode.T)) TestCombineModifiers();

        // 移除测试
        if (Input.GetKeyDown(KeyCode.Y)) TestRemoveLastModifier();
        if (Input.GetKeyDown(KeyCode.U)) ClearAllModifiers();

        // 实时更新UI
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (imStatText)
        {
            var hp = statController.GetImStat("Test_HP");
            if (hp != null)
                imStatText.text = $"[即时属性 HP]\n当前: {hp.CurrentValue:F1} / {hp.MaxValue:F1}\n基础: {hp.BaseValue:F1}";
            else
                imStatText.text = "[即时属性 HP]\n未找到";
        }

        if (passiveStatText)
        {
            var attack = statController.GetPassiveStat("Test_Attack");
            if (attack != null)
                passiveStatText.text = $"[被动属性 Attack]\n最终: {attack.FinalValue:F1}\n基础: {attack.BaseValue:F1}";
            else
                passiveStatText.text = "[被动属性 Attack]\n未找到";
        }
    }

    #region 即时属性测试

    /// <summary>
    /// 即时属性 FlatAdd - 直接加减当前值
    /// </summary>
    private void TestImStatFlatAdd()
    {
        Debug.Log("\n[即时属性] FlatAdd +30 (受伤30点)");
        _addedModifierIds.Clear(); // ImStat用ChangeValue，不需要记录modifier ID
        statController.ChangeAttributeValue("Test_HP", -30, E_ModifierType.FlatAdd);
    }

    /// <summary>
    /// 即时属性 PercentageAdd - 按比例变化
    /// </summary>
    private void TestImStatPercentageAdd()
    {
        Debug.Log("\n[即时属性] PercentageAdd -50% (损失一半)");
        _addedModifierIds.Clear();
        statController.ChangeAttributeValue("Test_HP", -50, E_ModifierType.PercentageAdd);
    }

    /// <summary>
    /// 即时属性 FinalAdd - 最终加减
    /// </summary>
    private void TestImStatFinalAdd()
    {
        Debug.Log("\n[即时属性] FinalAdd +50 (治疗50点)");
        _addedModifierIds.Clear();
        statController.ChangeAttributeValue("Test_HP", 50, E_ModifierType.FinalAdd);
    }

    /// <summary>
    /// 即时属性恢复 - 恢复到满血
    /// </summary>
    private void TestImStatHeal()
    {
        Debug.Log("\n[即时属性] 恢复到满血");
        var hp = statController.GetImStat("Test_HP");
        hp?.Restore();
    }

    #endregion

    #region 被动属性测试

    /// <summary>
    /// 被动属性 FlatAdd - 基础值加减
    /// 公式: Final = (Base + FlatAdd) * (1 + PercentAdd) + FinalAdd
    /// </summary>
    private void TestPassiveFlatAdd()
    {
        Debug.Log("\n[被动属性] FlatAdd +50 (攻击力+50)");
        string id = "FlatAdd_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        _addedModifierIds.Add(id);
        statController.AddModifier("Test_Attack", 
            new StatModifier(id, E_ModifierType.FlatAdd, 50f, this));
    }

    /// <summary>
    /// 被动属性 PercentageAdd - 百分比加成
    /// </summary>
    private void TestPassivePercentageAdd()
    {
        Debug.Log("\n[被动属性] PercentageAdd +100% (攻击力翻倍)");
        string id = "Percent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        _addedModifierIds.Add(id);
        statController.AddModifier("Test_Attack", 
            new StatModifier(id, E_ModifierType.PercentageAdd, 100f, this));
    }

    /// <summary>
    /// 被动属性 FinalAdd - 最终加减
    /// </summary>
    private void TestPassiveFinalAdd()
    {
        Debug.Log("\n[被动属性] FinalAdd +200 (攻击力+200)");
        string id = "FinalAdd_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        _addedModifierIds.Add(id);
        statController.AddModifier("Test_Attack", 
            new StatModifier(id, E_ModifierType.FinalAdd, 200f, this));
    }

    /// <summary>
    /// 被动属性 FinalPercentage - 最终百分比
    /// </summary>
    private void TestPassiveFinalPercentage()
    {
        Debug.Log("\n[被动属性] FinalPercentage +50% (最终伤害+50%)");
        string id = "FinalPercent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        _addedModifierIds.Add(id);
        statController.AddModifier("Test_Attack", 
            new StatModifier(id, E_ModifierType.FinalPercentage, 50f, this));
    }

    /// <summary>
    /// 组合测试 - 多种修饰符叠加
    /// </summary>
    private void TestCombineModifiers()
    {
        Debug.Log("\n[被动属性] 组合测试: +100攻击 -> +50% -> +100最终 -> +50%最终");
        
        // 基础100
        // FlatAdd +100 -> 200
        // PercentageAdd +50% -> 300
        // FinalAdd +100 -> 400
        // FinalPercentage +50% -> 600
        // 期望值: 600
        
        string id1 = "C_Flat_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        string id2 = "C_Percent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        string id3 = "C_FinalAdd_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        string id4 = "C_FinalPercent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);

        _addedModifierIds.AddRange(new[] { id1, id2, id3, id4 });

        statController.AddModifier("Test_Attack", new StatModifier(id1, E_ModifierType.FlatAdd, 100f, this));
        statController.AddModifier("Test_Attack", new StatModifier(id2, E_ModifierType.PercentageAdd, 50f, this));
        statController.AddModifier("Test_Attack", new StatModifier(id3, E_ModifierType.FinalAdd, 100f, this));
        statController.AddModifier("Test_Attack", new StatModifier(id4, E_ModifierType.FinalPercentage, 50f, this));

        Debug.Log($"期望值: 600 (100+100=200 -> *1.5=300 -> +100=400 -> *1.5=600)");
    }

    /// <summary>
    /// 移除最后一个修饰符
    /// </summary>
    private void TestRemoveLastModifier()
    {
        if (_addedModifierIds.Count == 0)
        {
            Debug.Log("[被动属性] 没有可移除的修饰符");
            return;
        }

        string id = _addedModifierIds[_addedModifierIds.Count - 1];
        _addedModifierIds.RemoveAt(_addedModifierIds.Count - 1);
        
        Debug.Log($"\n[被动属性] 移除修饰符: {id}");
        statController.GetPassiveStat("Test_Attack")?.RemoveModifier(id);
    }

    /// <summary>
    /// 清除所有修饰符
    /// </summary>
    private void ClearAllModifiers()
    {
        Debug.Log("\n[被动属性] 清除所有修饰符");
        _addedModifierIds.Clear();
        statController.GetPassiveStat("Test_Attack")?.ClearModifiers();
    }

    #endregion
}
