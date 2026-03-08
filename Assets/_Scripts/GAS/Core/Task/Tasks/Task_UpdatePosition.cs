using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// 跟随目标位置任务 - 暴风雪跟随玩家
    /// </summary>
    public class Task_UpdatePosition : AbilityTask
    {
        private Transform _target;
        private Vector3 _offset;
        private GameObject _effect;
        private float _updateInterval = 0.1f;
        private bool _keepYOffset = true;

        /// <summary>
        /// 创建跟随目标位置任务
        /// </summary>
        public static Task_UpdatePosition UpdatePosition(AbilitySystemComponent owner)
        {
            var task = new Task_UpdatePosition();
            task.Init(owner);
            return task;
        }

        public Task_UpdatePosition SetTarget(Transform target)
        {
            _target = target;
            return this;
        }

        public Task_UpdatePosition SetOffset(Vector3 offset)
        {
            _offset = offset;
            return this;
        }

        public Task_UpdatePosition SetEffect(GameObject effect)
        {
            _effect = effect;
            return this;
        }

        public Task_UpdatePosition SetUpdateInterval(float interval)
        {
            _updateInterval = interval;
            return this;
        }

        public Task_UpdatePosition SetKeepYOffset(bool keep)
        {
            _keepYOffset = keep;
            return this;
        }

        protected override async UniTask OnStart()
        {
            if (_target == null || _effect == null)
            {
                Debug.LogWarning("Task_UpdatePosition: Target or Effect is null!");
                EndTask();
                return;
            }

            while (_target != null && _effect != null && !IsEnded)
            {
                Vector3 targetPos = _target.position + _offset;

                if (_keepYOffset)
                {
                    targetPos.y = _effect.transform.position.y;
                }

                _effect.transform.position = targetPos;

                await UniTask.Delay((int)(_updateInterval * 1000), cancellationToken: CancellationToken);
            }

            EndTask();
        }

        public Transform GetTarget() => _target;
    }
}
