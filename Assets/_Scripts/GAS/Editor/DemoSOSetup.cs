using System;
using System.Collections.Generic;
using GAS.AbilitySystem;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    /// 一键创建所有Demo用的SO资源
    /// </summary>
    public class DemoSOSetup : EditorWindow
    {
        [MenuItem("GAS/Demo/创建所有SO")]
        public static void CreateAllSO()
        {
            // 创建目录（如果不存在）
            CreateDirectory("Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability");
            CreateDirectory("Assets/_Scripts/GAS/Demo/DemoScripts/So/GE");

            // 创建Ability SO
            CreateAbilitySO("Ability_LightningBolt", "LightningBolt");
            CreateAbilitySO("Ability_HealSelf", "HealSelf");
            CreateAbilitySO("Ability_SpeedBoost", "SpeedBoost");
            CreateAbilitySO("Ability_Blizzard", "Blizzard");

            // 创建GE SO (伤害=负值，治疗=正值)
            CreateGESO("GE_LightningDamage", -10f, E_EffectDuration.Instant);  // 伤害
            CreateGESO("GE_HealTick", 20f, E_EffectDuration.Instant);          // 治疗
            CreateGESO("GE_SpeedBoost", 5f, E_EffectDuration.HasDuration);       // 加速(持续)
            CreateGESO("GE_Blizzard", -5f, E_EffectDuration.HasDuration);       // 持续伤害

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("✅ 所有Demo SO创建完成！");
        }

        private static void CreateDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = "Assets";
                string folder = path.Replace("Assets/", "");
                string[] parts = folder.Split('/');
                string current = "Assets";
                foreach (var part in parts)
                {
                    if (!AssetDatabase.IsValidFolder(current + "/" + part))
                    {
                        AssetDatabase.CreateFolder(current, part);
                    }
                    current += "/" + part;
                }
            }
        }

        private static void CreateAbilitySO(string name, string abilityName)
        {
            string typeName = name.Replace("Ability_", "Ability_");
            System.Type abilityType = System.Type.GetType($"GAS.AbilitySystem.{typeName}, Assembly-CSharp");

            if (abilityType == null)
            {
                Debug.LogWarning($"⚠️ 未找到脚本类型: {typeName}，跳过创建 {name}");
                return;
            }

            string path = $"Assets/_Scripts/GAS/Demo/DemoScripts/So/Ability/{name}.asset";
            ScriptableObject ability = ScriptableObject.CreateInstance(abilityType);
            ability.name = name;

            // 设置默认属性
            var field = abilityType.GetField("abilityName");
            if (field != null)
            {
                field.SetValue(ability, abilityName);
            }

            AssetDatabase.CreateAsset(ability, path);
            Debug.Log($"✅ 创建 Ability SO: {name}");
        }

        private static void CreateGESO(string name, float value, E_EffectDuration durationType)
        {
            string path = $"Assets/_Scripts/GAS/Demo/DemoScripts/So/GE/{name}.asset";
            
            GameplayEffectData ge = ScriptableObject.CreateInstance<GameplayEffectData>();
            ge.name = name;
            
            // 创建修饰符配置
            string statId = value < 0 ? "HP" : "HP";  // 伤害和治疗都用HP
            E_ModifierType modType = value < 0 ? E_ModifierType.FlatAdd : E_ModifierType.FlatAdd;
            
            var modifier = new StatModifierConfig
            {
                statId = statId,
                type = modType,
                value = Mathf.Abs(value),
                priority = 0
            };
            
            // 使用反射设置私有字段
            SetPrivateField(ge, "duration", 5f);
            SetPrivateField(ge, "period", 1f);
            SetPrivateField(ge, "isPeriodic", durationType == E_EffectDuration.HasDuration);
            SetPrivateField(ge, "durationPolicy", durationType);
            SetPrivateField(ge, "statModifierConfig", new List<StatModifierConfig> { modifier });

            AssetDatabase.CreateAsset(ge, path);
            Debug.Log($"✅ 创建 GE SO: {name}");
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
