# 🎮 GAS Demo 计划书 - 火焰法师

> **项目路径**: `F:\AAAA学习资料\Demo\StatSystem`
> **创建日期**: 2026-03-08
> **目标**: 展示GAS核心功能 - GE/Ability/Task/Tag系统

---

## 📋 Demo概述

### 场景描述
- **类型**: 俯视角ARPG（类英雄联盟）
- **视角**: Top-Down Camera
- **控制**: 键盘移动 + 技能键释放技能
- **内容**: 单英雄控制 + 简单敌人AI

---

## 🏗️ 技术架构

### 需要的组件

```
GAS/Demo/
├── Character/
│   ├── HeroController.cs          # 玩家控制
│   └── EnemyAI.cs                 # 敌人AI
│
├── UI/
│   ├── SkillHUD.cs                # 技能图标UI
│   └── TargetingPreview.cs        # 范围预览
│
├── Ability/
│   ├── FireBallAbility.cs         # Q技能: 火球术
│   ├── FlameStormAbility.cs       # W技能: 烈焰风暴
│   ├── ShieldAbility.cs           # E技能: 护盾
│   └── BurnAbility.cs             # R技能: 燃烧
│
└── Input/
    └── InputManager.cs            # 输入管理
```

---

## 🎯 技能设计

### Q - 火球术 (单体 + DOT)

| 项目 | 内容 |
|------|------|
| **类型** | 单体敌人 |
| **消耗** | MP 20 |
| **冷却** | 3秒 |
| **效果** | 立即造成伤害 + 3秒DOT |

```csharp
// GE配置
- 分类标签: Skill.Damage, Damage.Fire
- 持续时间: 3秒
- 周期: 1秒
- 修饰符:
  - HP: -10/秒 (FlatAdd, DOT)
```

**展示点**:
- ✅ TargetType.SingleEnemy
- ✅ 周期性GE (Dot)
- ✅ Stat系统的持续伤害计算

---

### W - 烈焰风暴 (扇形AOE)

| 项目 | 内容 |
|------|------|
| **类型** | 扇形范围敌人 |
| **消耗** | MP 35 |
| **冷却** | 5秒 |
| **效果** | 立即造成范围伤害 |

```csharp
// GE配置
- 分类标签: Skill.Damage, Damage.Fire, Skill.AoE
- 持续时间: 即时
- 修饰符:
  - HP: -50 (FlatAdd)
```

**展示点**:
- ✅ TargetType.ConeEnemy
- ✅ 扇形范围检测
- ✅ 多个目标同时应用GE

---

### E - 护盾 (属性Buff)

| 项目 | 内容 |
|------|------|
| **类型** | 自身 |
| **消耗** | MP 30 |
| **冷却** | 8秒 |
| **效果** | 5秒内提升防御和攻速 |

```csharp
// GE配置
- 分类标签: Skill.Buff, Buff.Defense
- 持续时间: 5秒
- 修饰符:
  - Defense: +20 (FlatAdd)
  - AttackSpeed: +10% (PercentageAdd)
```

**展示点**:
- ✅ TargetType.Self
- ✅ GameplayEffect的Buff效果
- ✅ 多个属性同时修改
- ✅ 正面效果展示

---

### R - 燃烧 (Debuff + DOT + Tag前提)

| 项目 | 内容 |
|------|------|
| **类型** | 圆形范围敌人 |
| **消耗** | MP 80 |
| **冷却** | 30秒 |
| **效果** | 大范围DOT + 减速 + 前提条件 |

```csharp
// GE配置
- 分类标签: Skill.Ultimate, Debuff.Burn
- 前提条件: 需要标签 State.InCombat (可配置)
- 持续时间: 4秒
- 周期: 0.5秒
- 修饰符:
  - HP: -15/0.5秒 (FlatAdd)
  - MoveSpeed: -20% (PercentageAdd, 减速)
```

**展示点**:
- ✅ Tag前提条件检查
- ✅ 周期DOT + 属性减速
- ✅ 大招/终极技能概念
- ✅ AreaEnemy范围

---

## 📊 GE配置总表

| 技能 | GE名称 | 持续时间 | 周期 | 效果 |
|------|--------|----------|------|------|
| Q | GE_Fireball | 3秒 | 1秒 | -10HP/s DOT |
| Q | GE_Fireball_Direct | 即时 | - | -30HP 即时伤害 |
| W | GE_FlameStorm | 即时 | - | -50HP AOE |
| E | GE_Shield | 5秒 | - | +20防御, +10%攻速 |
| R | GE_Burn | 4秒 | 0.5秒 | -15HP/s + -20%移速 |

---

## 🎮 控制方式

### 键盘
| 按键 | 功能 |
|------|------|
| W/A/S/D | 移动 |
| Q | 释放火球术 |
| W | 释放烈焰风暴 |
| E | 释放护盾 |
| R | 释放燃烧 |

### 鼠标
| 操作 | 功能 |
|------|------|
| 鼠标移动 | 瞄准方向 |
| 左键点击 | 确认释放技能 |

---

## 🔧 实现步骤

### 阶段1: 基础搭建
- [ ] 创建Demo场景
- [ ] 添加地形和相机
- [ ] 创建Hero预制体

### 阶段2: 角色控制
- [ ] 实现HeroController移动
- [ ] 实现鼠标瞄准
- [ ] 实现InputManager

### 阶段3: 技能系统
- [ ] 创建4个Ability SO
- [ ] 创建对应的GE配置 SO
- [ ] 实现技能释放逻辑

### 阶段4: 目标选择
- [ ] 实现范围预览UI
- [ ] 集成TargetingSystem
- [ ] 显示不同范围特效

### 阶段5: 敌人AI
- [ ] 创建敌人预制体
- [ ] 简单追逐AI
- [ ] 受伤反馈

---

## ✅ 功能检查清单

- [ ] GE系统 - 伤害/Buff/Debuff/DOT
- [ ] Stat系统 - 属性修饰符计算
- [ ] Targeting系统 - 单体/扇形/圆形范围
- [ ] Tag系统 - 分类标签和前提条件
- [ ] Ability系统 - 冷却和消耗
- [ ] Task系统 - 异步任务支持

---

## 📝 后续扩展

1. **更多技能**: 闪现、治疗、召唤物
2. **敌人类型**: 战士、法师、坦克
3. **装备系统**: 武器、防具、饰品
4. **等级系统**: 经验获取和升级

---

> 📝 **更新日期**: 2026-03-08
> 🎯 **目标**: 完成可运行的Demo，展示GAS核心功能
