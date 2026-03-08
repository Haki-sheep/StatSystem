using System.Collections;
using GAS.Component;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GAS.Test
{
    /// <summary>
    /// AbilitySystemComponent 测试脚本
    /// 提供 UI 按钮来测试各种 GE 效果
    /// </summary>
    public class ASCTest : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private AbilitySystemComponent abilitySystem;
        [SerializeField] private StatController statController;

        [Header("GE 配置")]
        [SerializeField] private GameplayEffectData instantHeal;      // 即时治疗
        [SerializeField] private GameplayEffectData instantDamage;   // 即时伤害
        [SerializeField] private GameplayEffectData durationBuff;    // 持续 Buff (攻击力)
        [SerializeField] private GameplayEffectData durationDebuff;  // 持续 Debuff (减速)
        [SerializeField] private GameplayEffectData dotEffect;       // 持续伤害 (Dot)
        [SerializeField] private GameplayEffectData hotEffect;       // 持续治疗 (Hot)
        [SerializeField] private GameplayEffectData stackingBuff;    // 可堆叠 Buff

        [Header("UI 引用")]
        [SerializeField] private Text healthText;
        [SerializeField] private Text attackText;
        [SerializeField] private Text defenseText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text logText;

        private void Start()
        {
            // 初始化属性控制器
            statController.Init();

            // 打印初始属性
            Log("=== 测试开始 ===");
            PrintAllStats();
        }

        private void Update()
        {
            // 每帧更新 UI 显示
            UpdateUI();
        }

        #region UI 更新

        private void UpdateUI()
        {
            if (healthText && statController)
            {
                healthText.text = $"HP: {statController.GetCurrentValue("HP"):F1} / {statController.GetValue("HP"):F1}";
            }

            if (attackText && statController)
            {
                attackText.text = $"Attack: {statController.GetCurrentValue("Attack"):F1}";
            }

            if (defenseText && statController)
            {
                defenseText.text = $"Defense: {statController.GetCurrentValue("Defense"):F1}";
            }

            if (speedText && statController)
            {
                speedText.text = $"Speed: {statController.GetCurrentValue("Speed"):F1}";
            }
        }

        #endregion

        #region 测试方法

        /// <summary>
        /// 即时治疗
        /// </summary>
        [Button("即时治疗 +100HP")]
        public void TestInstantHeal()
        {
            if (instantHeal == null)
            {
                Log("错误: 未配置 instantHeal");
                return;
            }

            var spec = abilitySystem.ApplyGE(instantHeal, this);
            Log($"[即时治疗] 应用成功, HP 变化: +{instantHeal.StatModifierConfig[0].value}");
            PrintAllStats();
        }

        /// <summary>
        /// 即时伤害
        /// </summary>
        [Button("即时伤害 -50HP")]
        public void TestInstantDamage()
        {
            if (instantDamage == null)
            {
                Log("错误: 未配置 instantDamage");
                return;
            }

            var spec = abilitySystem.ApplyGE(instantDamage, this);
            Log($"[即时伤害] 应用成功, HP 变化: {instantDamage.StatModifierConfig[0].value}");
            PrintAllStats();
        }

        /// <summary>
        /// 持续 Buff - 攻击力提升
        /// </summary>
        [Button("持续Buff 攻击+50")]
        public void TestDurationBuff()
        {
            if (durationBuff == null)
            {
                Log("错误: 未配置 durationBuff");
                return;
            }

            var spec = abilitySystem.ApplyGE(durationBuff, this);
            Log($"[持续Buff] 应用成功, Attack +{durationBuff.StatModifierConfig[0].value}, 持续 {durationBuff.DurationValue}s");
            PrintAllStats();
        }

        /// <summary>
        /// 持续 Debuff - 减速效果
        /// </summary>
        [Button("持续Debuff 减速30%")]
        public void TestDurationDebuff()
        {
            if (durationDebuff == null)
            {
                Log("错误: 未配置 durationDebuff");
                return;
            }

            var spec = abilitySystem.ApplyGE(durationDebuff, this);
            Log($"[持续Debuff] 应用成功, Speed {durationDebuff.StatModifierConfig[0].value}%, 持续 {durationDebuff.DurationValue}s");
            PrintAllStats();
        }

        /// <summary>
        /// 持续伤害 (Dot)
        /// </summary>
        [Button("Dot 毒药伤害")]
        public void TestDotEffect()
        {
            if (dotEffect == null)
            {
                Log("错误: 未配置 dotEffect");
                return;
            }

            var spec = abilitySystem.ApplyGE(dotEffect, this);
            Log($"[Dot] 应用成功, 每 {dotEffect.Period}s 造成 {dotEffect.StatModifierConfig[0].value} 伤害");
            PrintAllStats();
        }

        /// <summary>
        /// 持续治疗 (Hot)
        /// </summary>
        [Button("Hot 生命恢复")]
        public void TestHotEffect()
        {
            if (hotEffect == null)
            {
                Log("错误: 未配置 hotEffect");
                return;
            }

            var spec = abilitySystem.ApplyGE(hotEffect, this);
            Log($"[Hot] 应用成功, 每 {hotEffect.Period}s 恢复 {hotEffect.StatModifierConfig[0].value} HP");
            PrintAllStats();
        }

        /// <summary>
        /// 可堆叠 Buff
        /// </summary>
        [Button("堆叠Buff 攻击+10%/层")]
        public void TestStackingBuff()
        {
            if (stackingBuff == null)
            {
                Log("错误: 未配置 stackingBuff");
                return;
            }

            var spec = abilitySystem.ApplyGE(stackingBuff, this);
            Log($"[堆叠Buff] 应用成功, 当前层数: {spec.StackCount}, 最大: {stackingBuff.StackLimit}");
            PrintAllStats();
        }

        /// <summary>
        /// 打印所有属性
        /// </summary>
        [Button("打印属性")]
        public void PrintAllStats()
        {
            if (statController == null) return;

            float hp = statController.GetCurrentValue("HP");
            float baseHp = statController.GetValue("HP");
            float attack = statController.GetCurrentValue("Attack");
            float defense = statController.GetCurrentValue("Defense");
            float speed = statController.GetCurrentValue("Speed");

            Log($"属性: HP={hp:F1}/{baseHp:F1}, Attack={attack:F1}, Defense={defense:F1}, Speed={speed:F1}");
        }

        #endregion

        #region 日志

        private void Log(string message)
        {
            Debug.Log(message);
            if (logText)
            {
                logText.text = message + "\n" + logText.text;
            }
        }

        #endregion
    }
}
