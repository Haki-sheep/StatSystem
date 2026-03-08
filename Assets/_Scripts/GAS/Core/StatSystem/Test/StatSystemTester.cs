// using GAS.Core.StateSystem;
// using UnityEngine;

// /// <summary>
// /// StatSystem 测试脚本
// /// 使用方法：
// /// 1. 创建空 GameObject，添加此组件
// /// 2. 将 StatDefinition1 拖入 Stat Data List
// /// 3. 运行游戏，按下按键测试
// /// </summary>
// public class StatSystemTester : MonoBehaviour
// {
//     [Header("属性数据")]
//     [SerializeField] private StatData[] statDataList;

//     private StatController statController;

//     private void Awake()
//     {
//         statController = gameObject.AddComponent<StatController>();
//         SetStatDataList(statDataList);
//         statController.Init();
//     }

//     private void SetStatDataList(StatData[] dataList)
//     {
//         var field = typeof(StatController).GetField("statDataList", 
//             System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
//         field?.SetValue(statController, new System.Collections.Generic.List<StatData>(dataList));
//     }

//     private void Start()
//     {
//         Debug.Log("=== StatSystem 测试开始 ===");
//         PrintStats();
//     }

//     private void PrintStats()
//     {
//         Debug.Log($"\n--- StatDefinition1 ---");
//         var stat = statController.GetStat("StatDefinition1");
//         // Debug.Log($"Base={stat.BaseValue}, Final={stat.FinalValue}, Current={attr?.CurrentValue}");
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Alpha1)) TestFlatAdd();
//         if (Input.GetKeyDown(KeyCode.Alpha2)) TestPercentageAdd();
//         if (Input.GetKeyDown(KeyCode.Alpha3)) TestFinalAdd();
//         if (Input.GetKeyDown(KeyCode.Alpha4)) TestFinalPercentage();
//         if (Input.GetKeyDown(KeyCode.Alpha5)) TestRemoveSource();
//         if (Input.GetKeyDown(KeyCode.Alpha6)) TestCombine();
//         if (Input.GetKeyDown(KeyCode.R)) TestReset();
//     }

//     /// <summary>
//     /// 测试：加减（FlatAdd）
//     /// 公式: result = baseValue + flatAdd
//     /// </summary>
//     private void TestFlatAdd()
//     {
//         Debug.Log("\n[测试] 添加 FlatAdd +20");
//         string id = "FlatAdd_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id, this, E_ModifierType.FlatAdd, 20f));
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：百分比（PercentageAdd）
//     /// 公式: result = (baseValue + flatAdd) * (1 + percentageAdd/100)
//     /// </summary>
//     private void TestPercentageAdd()
//     {
//         Debug.Log("\n[测试] 添加 PercentageAdd +50%");
//         string id = "Percent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id, this, E_ModifierType.PercentageAdd, 50f));
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：最终加减（FinalAdd）
//     /// 公式: result = (baseValue + flatAdd) * (1 + percentageAdd/100) + finalAdd
//     /// </summary>
//     private void TestFinalAdd()
//     {
//         Debug.Log("\n[测试] 添加 FinalAdd +100");
//         string id = "FinalAdd_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id, this, E_ModifierType.FinalAdd, 100f));
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：最终百分比（FinalPercentage）
//     /// 公式: result = ((baseValue + flatAdd) * (1 + percentageAdd/100) + finalAdd) * (1 + finalPercentage/100)
//     /// </summary>
//     private void TestFinalPercentage()
//     {
//         Debug.Log("\n[测试] 添加 FinalPercentage +50%");
//         string id = "FinalPercent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id, this, E_ModifierType.FinalPercentage, 50f));
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：组合使用
//     /// </summary>
//     private void TestCombine()
//     {
//         Debug.Log("\n[测试] 组合使用");
//         // 先加100攻击，再加50%攻击力，最终+50%
//         // 结果 = (100 + 100) * (1 + 50/100) * (1 + 0/100) = 200 * 1.5 = 300
//         string id1 = "Combine_Flat_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
//         string id2 = "Combine_Percent_" + System.Guid.NewGuid().ToString("N").Substring(0, 4);
        
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id1, this, E_ModifierType.FlatAdd, 100f));
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(id2, this, E_ModifierType.PercentageAdd, 50f));
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：移除来源的所有修饰符
//     /// </summary>
//     private void TestRemoveSource()
//     {
//         Debug.Log("\n[测试] 移除来源的所有修饰符");
//         // 先添加多个修饰符
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(this, E_ModifierType.FlatAdd, 10f));
//         statController.AddModifier("StatDefinition1", new StatModifier<StatSystemTester>(this, E_ModifierType.PercentageAdd, 20f));
//         Debug.Log("添加后:");
//         PrintStats();
        
//         statController.RemoveModifiersFromSource("StatDefinition1", this);
//         Debug.Log("移除后:");
//         PrintStats();
//     }

//     /// <summary>
//     /// 测试：重置
//     /// </summary>
//     private void TestReset()
//     {
//         Debug.Log("\n=== 重置 ===");
//         var stat = statController.GetStat("StatDefinition1");
//         stat.ClearModifiers();
//         stat?.Restore();
//         PrintStats();
//     }
// }
