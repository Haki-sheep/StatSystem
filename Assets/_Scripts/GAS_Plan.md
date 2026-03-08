🎮 纯手搓GAS实施计划书

> 参考来源：
> - **Effect系统**：UE5 GAS 标准（GameplayEffect）
> - **其他系统**：`C:\Users\fmz\Downloads\ability-system-course-main\ability-system-course-main\Assets`
> 
> 项目路径：`F:\AAAA学习资料\Demo\StatSystem`

---

📋 概述

本计划书旨在从零开始手写一套轻量级 **Gameplay Ability System (GAS)**，不依赖网络同步、编辑器节点拓展、动画通知等功能，专注于单人对战或PVE场景。

**核心原则：**
- 保持轻量，不做过度设计
- 参考课程架构，但代码完全手写
- **Effect系统以UE5 GAS为标准**（共享计时器堆叠）
- 先跑通核心功能，再逐步完善

---

🏗️ 整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                        AbilitySystemComponent                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────┐   ┌─────────────┐   ┌─────────────┐           │
│  │  Gameplay   │   │   Gameplay  │   │   Gameplay  │           │
│  │   Ability   │ ← │   Effect    │ ← │   Tags      │           │
│  │  (技能)      │   │  (Buff/属性) │   │  (标签)    │           │
│  └─────────────┘   └─────────────┘   └─────────────┘           │
│         ↑                 ↑                                    │
│         │                 │                                    │
│  ┌─────────────┐   ┌─────────────┐                            │
│  │ AbilityTask │   │  TargetData │                            │
│  │ (异步任务)   │   │ (目标选择)   │                            │
│  └─────────────┘   └─────────────┘                            │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

📁 文件结构

```
Assets/_Scripts/GAS/
├── Component/
│   └── AbilitySystemComponent.cs         ✅ 已完成（重构）
│
├── Core/
│   ├── TagSystem/                         ✅ 已完成
│   │   ├── GameplayTag.cs
│   │   ├── GameplayTagContainer.cs
│   │   └── GameplayTagRequirements.cs
│   │
│   ├── StatSystem/                        ✅ 已完成
│   │   ├── IStat.cs                      （接口分离：IStat/IPassiveStat/IImmediateStat）
│   │   ├── Stat.cs                       （被动属性）
│   │   ├── ImStat.cs                     （即时属性）
│   │   ├── StatData.cs
│   │   ├── StatModifier.cs
│   │   └── StatController.cs
│   │
│   ├── Effect/                            ✅ 已完成
│   │   ├── GameplayEffectData.cs         （效果定义）
│   │   ├── GameplayEffectSpec.cs         （效果实例）
│   │   ├── GameplayEffectEnums.cs        （枚举）
│   │   ├── GETimerManager.cs             （Tick管理器）
│   │   └── GEManager.cs                  （GE生命周期管理）
│   │
│   └── Ability/                           ✅ 已完成
│       ├── GameplayAbility.cs            （技能基类）
│       ├── AbilitySpec.cs                （技能实例）
│       ├── AbilityCost.cs                 （技能消耗）
│       ├── AbilityCooldown.cs            （技能冷却）
│       └── AbsTest/
│           ├── AbsTest.cs
│           └── MyAbility.cs
│
├── Task/                                  🔄 待实现
│   ├── AbilityTask.cs                    任务基类
│   └── Tasks/
│       ├── Task_Wait.cs
│       ├── Task_SpawnEffect.cs
│       └── Task_WaitTargetData.cs
│
└── Targeting/                            🔄 待实现
    ├── TargetData.cs
    ├── TargetType.cs
    └── TargetingSystem.cs
```

---

✅ 已完成模块

### 1. Tag 系统

| 文件 | 状态 |
|------|------|
| `GameplayTag.cs` | ✅ |
| `GameplayTagContainer.cs` | ✅ |
| `GameplayTagRequirements.cs` | ✅ |

### 2. Stat 系统

| 文件 | 说明 |
|------|------|
| `IStat.cs` | ✅ 接口分离（IStat/IPassiveStat/IImmediateStat） |
| `Stat.cs` | ✅ 被动属性（修饰符系统） |
| `ImStat.cs` | ✅ 即时属性（直接修改值） |
| `StatData.cs` | ✅ |
| `StatModifier.cs` | ✅ |
| `StatController.cs` | ✅ |

> **计算方式**：4阶段计算（FlatAdd → PercentageAdd → FinalAdd → FinalPercentage）

### 3. GameplayEffect 系统

| 文件 | 说明 |
|------|------|
| `GameplayEffectData.cs` | ✅ 效果定义（SO） |
| `GameplayEffectSpec.cs` | ✅ 效果实例 |
| `GameplayEffectEnums.cs` | ✅ 枚举定义 |
| `GETimerManager.cs` | ✅ Tick 管理器 |
| `GEManager.cs` | ✅ GE 生命周期管理 |

### 4. Ability 系统

| 文件 | 说明 |
|------|------|
| `GameplayAbility.cs` | ✅ 技能基类（SO） |
| `AbilitySpec.cs` | ✅ 技能实例 |
| `AbilityCost.cs` | ✅ 技能消耗 |
| `AbilityCooldown.cs` | ✅ 技能冷却 |

### 5. AbilitySystemComponent

| 文件 | 说明 |
|------|------|
| `AbilitySystemComponent.cs` | ✅ 已重构为协调者模式 |

---

🔄 待实现模块

### 📊 进度总览

| 阶段 | 模块 | 状态 |
|------|------|------|
| 第一阶段 | Tag系统 | ✅ 已完成 |
| 第一阶段 | Stat系统 | ✅ 已完成 |
| 第二阶段 | GameplayEffect | ✅ 已完成 |
| 第三阶段 | AbilitySystemComponent | ✅ 已完成 |
| 第四阶段 | Ability技能 | ✅ 已完成 |
| 第五阶段 | TargetData | ✅ 已完成 |
| 第六阶段 | AbilityTask | ✅ 已完成 |
| 第七阶段 | Demo测试 | 🔄 待实现 |

---

---

> 📝 **更新日期**：2026-03-07
> 
> 🎯 **目标**：完成一个可运行的GAS核心 + 3个Demo技能
