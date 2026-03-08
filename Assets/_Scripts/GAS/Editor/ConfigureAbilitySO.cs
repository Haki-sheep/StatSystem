using System.Collections.Generic;
using GAS.AbilitySystem;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    /// 配置所有Demo Ability SO的 Inspector 字段
    /// </summary>
    public class ConfigureAbilitySO : EditorWindow
    {
        [MenuItem("GAS/Demo/配置Ability SO")]
        public static void Configure()
        {
            // ========== 配置 Ability_LightningBolt ==========
            var lightningBolt = AssetDatabase.LoadAssetAtPath<Ability_LightningBolt>(
                "Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability/Ability_LightningBolt.asset");
            if (lightningBolt != null)
            {
                lightningBolt.abilityName = "LightningBolt";
                lightningBolt.cooldownTime = 2f;
                // ProjectilePrefab, HitEffectPrefab, DamageEffect 需手动拖入
                lightningBolt.projectileSpeed = 15f;
                lightningBolt.delayBeforeLaunch = 0.5f;
                lightningBolt.arriveDistance = 1f;
                EditorUtility.SetDirty(lightningBolt);
                Debug.Log("✅ 配置 Ability_LightningBolt");
            }

            // ========== 配置 Ability_HealSelf ==========
            var healSelf = AssetDatabase.LoadAssetAtPath<Ability_HealSelf>(
                "Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability/Ability_HealSelf.asset");
            if (healSelf != null)
            {
                healSelf.abilityName = "HealSelf";
                healSelf.cooldownTime = 5f;
                // HealEffect, HealEffectPrefab 需手动拖入
                EditorUtility.SetDirty(healSelf);
                Debug.Log("✅ 配置 Ability_HealSelf");
            }

            // ========== 配置 Ability_SpeedBoost ==========
            var speedBoost = AssetDatabase.LoadAssetAtPath<Ability_SpeedBoost>(
                "Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability/Ability_SpeedBoost.asset");
            if (speedBoost != null)
            {
                speedBoost.abilityName = "SpeedBoost";
                speedBoost.cooldownTime = 10f;
                // SpeedBoostEffect, SpeedEffectPrefab 需手动拖入
                EditorUtility.SetDirty(speedBoost);
                Debug.Log("✅ 配置 Ability_SpeedBoost");
            }

            // ========== 配置 Ability_Blizzard ==========
            var blizzard = AssetDatabase.LoadAssetAtPath<Ability_Blizzard>(
                "Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability/Ability_Blizzard.asset");
            if (blizzard != null)
            {
                blizzard.abilityName = "Blizzard";
                blizzard.cooldownTime = 15f;
                blizzard.blizzardRadius = 5f;
                // BlizzardEffectPrefab, EnemyLayer, BlizzardDamageEffect 需手动设置
                blizzard.damageInterval = 0.5f;
                EditorUtility.SetDirty(blizzard);
                Debug.Log("✅ 配置 Ability_Blizzard");
            }

            AssetDatabase.SaveAssets();
            Debug.Log("✅ 所有SO配置完成！");
        }

        private static void ConfigureGESO(string geName, string statId, float value, 
            E_EffectDuration durationType, bool isPeriodic, float period)
        {
            string path = $"Assets/_Scripts/GAS/Demo/DemoScripts/So/GE/{geName}.asset";
            var ge = AssetDatabase.LoadAssetAtPath<GameplayEffectData>(path);
            if (ge == null) return;

            var modifier = new StatModifierConfig
            {
                statId = statId,
                type = E_ModifierType.FlatAdd,
                value = Mathf.Abs(value),
                priority = 0
            };

            SetPrivateField(ge, "durationPolicy", durationType);
            SetPrivateField(ge, "duration", 5f);
            SetPrivateField(ge, "isPeriodic", isPeriodic);
            SetPrivateField(ge, "period", period);
            SetPrivateField(ge, "statModifierConfig", new List<StatModifierConfig> { modifier });

            EditorUtility.SetDirty(ge);
            Debug.Log($"✅ 配置 {geName}");
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
