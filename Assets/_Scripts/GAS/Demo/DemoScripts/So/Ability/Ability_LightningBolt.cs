using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.Core;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.AbilitySystem
{
    /// <summary>
    /// 雷电球技能 - 创建后0.5s飞向敌人，遇到敌人销毁并造成伤害
    /// </summary>
    [CreateAssetMenu(fileName = "Ability_LightningBolt", menuName = "GAS/Demo/Ability_LightningBolt")]
    public class Ability_LightningBolt : GameplayAbility
    {
        [Header("雷电球设置")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] public float projectileSpeed = 15f;
        [SerializeField] public float delayBeforeLaunch = 0.5f;
        [SerializeField] public float arriveDistance = 1f;

        [Header("伤害设置")]
        [SerializeField] private GameplayEffectData damageEffect;

        // 任务引用
        private Task_MoveToTarget _moveTask;

        public override void Activate(StatController statController)
        {
            Debug.Log($"[雷电球] 激活技能");

            // 获取目标
            var player = statController.GetComponent<Demo_Player>();
            if (player == null)
            {
                Debug.LogWarning("[雷电球] 未找到Demo_Player组件!");
                return;
            }

            Transform target = player.GetCurrentTarget();
            if (target == null)
            {
                Debug.LogWarning("[雷电球] 未选中目标!");
                return;
            }

            // 获取ASC
            var asc = player.GetAbilitySystemComponent();
            if (asc == null)
            {
                Debug.LogWarning("[雷电球] 未找到AbilitySystemComponent!");
                return;
            }

            // 启动异步任务
            ExecuteLightningBolt(asc, target).Forget();
        }

        private async UniTask ExecuteLightningBolt(AbilitySystemComponent asc, Transform target)
        {
            // 1. 在玩家前方生成投射物
            var ownerTransform = asc.transform;
            // 计算生成位置：玩家位置 + 玩家朝向 * 1.5f（前方一点）
            Vector3 spawnPosition = ownerTransform.position + ownerTransform.forward * 1.5f;
            spawnPosition.y += 1f; // 稍微抬高一点
            
            GameObject projectile = null;

            var spawnTask = Task_SpawnEffect.SpawnEffect(asc)
                .SetEffectPrefab(projectilePrefab)
                .SetLocation(spawnPosition)
                .SetDuration(-1); // 不自动销毁
            spawnTask.Start();
            await UniTask.WaitUntil(() => spawnTask.IsEnded, cancellationToken: spawnTask.CancellationToken);
            projectile = spawnTask.SpawnedEffect;

            // 设置投射物朝向目标方向
            if (projectile != null && target != null)
            {
                projectile.transform.LookAt(target);
            }

            if (projectile == null || target == null)
            {
                return;
            }

            // 2. 等待0.5秒
            await UniTask.Delay((int)(delayBeforeLaunch * 1000), cancellationToken: asc.GetCancellationTokenOnDestroy());

            if (projectile == null || target == null)
            {
                return;
            }

            // 3. 飞向目标
            _moveTask = Task_MoveToTarget.MoveToTarget(asc)
                .SetTarget(target)
                .SetProjectile(projectile)
                .SetSpeed(projectileSpeed)
                .SetArriveDistance(arriveDistance)
                .SetDestroyOnArrive(false);
            _moveTask.Start();
            await UniTask.WaitUntil(() => _moveTask.IsEnded, cancellationToken: asc.GetCancellationTokenOnDestroy());

            // 4. 命中目标
            if (projectile != null && target != null)
            {
                // 应用伤害GE
                var enemyASC = target.GetComponent<AbilitySystemComponent>();
                if (enemyASC != null && damageEffect != null)
                {
                    enemyASC.ApplyGE(damageEffect, asc.transform);
                }

                // 生成命中特效
                var hitTask = Task_SpawnEffect.SpawnEffect(asc)
                    .SetEffectPrefab(hitEffectPrefab)
                    .SetTarget(target)
                    .SetDuration(0.5f);
                hitTask.Start();

                // 销毁投射物
                Destroy(projectile);
            }
        }

        public override void InterruptTask()
        {
            base.InterruptTask();
            if (_moveTask != null)
            {
                _moveTask.InterruptTask();
            }
        }
    }
}
