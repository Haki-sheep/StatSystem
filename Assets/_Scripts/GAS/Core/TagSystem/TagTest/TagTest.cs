using UnityEngine;
using GAS.TagSystem;

public class TagTest : MonoBehaviour
{
    void Start()
    {
        Test_ParentChild();
        Test_Container();
        Test_Requirements();
    }

    void Test_ParentChild()
    {
        Debug.Log("=== 父子关系测试 ===");
        var ailment = new GameplayTag("Ailment");
        var poison = new GameplayTag("Ailment.Poison");
        var burn = new GameplayTag("Ailment.Burn");
        var health = new GameplayTag("Health");

        Debug.Log($"Ailment 是 Ailment.Poison 的父标签: {ailment.IsParentOf(poison)}"); // True
        Debug.Log($"Ailment.Poison 是 Ailment 的父标签: {poison.IsParentOf(ailment)}"); // False
        Debug.Log($"Ailment 是 Ailment.Burn 的父标签: {ailment.IsParentOf(burn)}");   // True
        Debug.Log($"Health 是 Ailment 的父标签: {health.IsParentOf(ailment)}");       // False
    }

    void Test_Container()
    {
        Debug.Log("=== 容器测试 ===");
        var container = new GameplayTagContainer();

        // 添加标签
        container.AddTags(
            new GameplayTag("Player"),
            new GameplayTag("Ailment.Poison"),
            new GameplayTag("Damage.Fire")
        );

        // 精确匹配
        Debug.Log($"包含Player: {container.Contains(new GameplayTag("Player"))}");       // True
        Debug.Log($"包含Ailment: {container.Contains(new GameplayTag("Ailment"))}");     // False

        // 父子匹配(ContainsTag)
        Debug.Log($"包含Ailment(父子): {container.ContainsTag(new GameplayTag("Ailment"))}");         // True (Ailment.Poison)
        Debug.Log($"包含Ailment.Poison(父子): {container.ContainsTag(new GameplayTag("Ailment.Poison"))}"); // True

        // ContainsAny / ContainsAll
        var checkTags = new[] { new GameplayTag("Player"), new GameplayTag("Enemy") };
        Debug.Log($"包含任一: {container.ContainsAny(checkTags)}");  // True (有Player)

        var needTags = new[] { new GameplayTag("Player"), new GameplayTag("Ailment.Poison") };
        Debug.Log($"包含全部: {container.ContainsAll(needTags)}");  // True
    }

    void Test_Requirements()
    {
        Debug.Log("=== 黑白名单测试 ===");
        
        // 需求：需要Player标签，禁止Enemy标签
        var req = new GameplayTagRequirements();
        req.NeedTags.AddTag(new GameplayTag("Player"));
        req.BanTags.AddTag(new GameplayTag("Enemy"));

        // 测试1：有Player，无Enemy → 通过
        var tags1 = new GameplayTagContainer();
        tags1.AddTag(new GameplayTag("Player"));
        Debug.Log($"有Player无Enemy: {req.IsSatisfied(tags1)}");  // True

        // 测试2：没有Player → 失败
        var tags2 = new GameplayTagContainer();
        tags2.AddTag(new GameplayTag("Enemy"));
        Debug.Log($"无Player有Enemy: {req.IsSatisfied(tags2)}");  // False

        // 测试3：有Player和Enemy → 失败(被黑名单拦截)
        var tags3 = new GameplayTagContainer();
        tags3.AddTags(new GameplayTag("Player"), new GameplayTag("Enemy"));
        Debug.Log($"有Player有Enemy: {req.IsSatisfied(tags3)}");  // False
    }
}