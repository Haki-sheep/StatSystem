using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// 移动到目标任务 - 投射物飞向目标
    /// </summary>
    public class Task_MoveToTarget : AbilityTask
    {
        private Transform _target;
        private float _speed = 10f;
        private float _arriveDistance = 0.5f;
        private GameObject _projectile;
        private bool _destroyOnArrive = true;

        /// <summary>
        /// 创建移动到目标任务
        /// </summary>
        public static Task_MoveToTarget MoveToTarget(AbilitySystemComponent owner)
        {
            var task = new Task_MoveToTarget();
            task.Init(owner);
            return task;
        }

        public Task_MoveToTarget SetTarget(Transform target)
        {
            _target = target;
            return this;
        }

        public Task_MoveToTarget SetSpeed(float speed)
        {
            _speed = speed;
            return this;
        }

        public Task_MoveToTarget SetArriveDistance(float distance)
        {
            _arriveDistance = distance;
            return this;
        }

        public Task_MoveToTarget SetProjectile(GameObject projectile)
        {
            _projectile = projectile;
            return this;
        }

        public Task_MoveToTarget SetDestroyOnArrive(bool destroy)
        {
            _destroyOnArrive = destroy;
            return this;
        }

        protected override async UniTask OnStart()
        {
            if (_target == null || _projectile == null)
            {
                Debug.LogWarning("Task_MoveToTarget: Target or Projectile is null!");
                EndTask();
                return;
            }

            while (_target != null && _projectile != null && !IsEnded)
            {
                // 计算方向和距离
                Vector3 direction = (_target.position - _projectile.transform.position).normalized;
                float distance = Vector3.Distance(_projectile.transform.position, _target.position);

                // 到达目标
                if (distance <= _arriveDistance)
                {
                    break;
                }

                // 移动
                _projectile.transform.position += direction * _speed * Time.deltaTime;
                _projectile.transform.LookAt(_target);

                await UniTask.Yield();
            }

            EndTask();
        }

        public Transform GetTarget() => _target;
    }
}
