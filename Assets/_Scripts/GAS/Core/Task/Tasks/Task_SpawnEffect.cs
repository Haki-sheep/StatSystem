using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// 生成效果任务 - 在目标位置生成效果
    /// </summary>
    public class Task_SpawnEffect : AbilityTask
    {
        private GameObject _effectPrefab;
        private Vector3 _location;
        private Transform _target;
        private float _duration = -1f; // -1 表示不自动销毁
        private GameObject _spawnedEffect;

        /// <summary>
        /// 创建生成效果任务
        /// </summary>
        public static Task_SpawnEffect SpawnEffect(AbilitySystemComponent owner)
        {
            var task = new Task_SpawnEffect();
            task.Init(owner);
            return task;
        }

        public Task_SpawnEffect SetEffectPrefab(GameObject prefab)
        {
            _effectPrefab = prefab;
            return this;
        }

        public Task_SpawnEffect SetLocation(Vector3 location)
        {
            _location = location;
            return this;
        }

        public Task_SpawnEffect SetTarget(Transform target)
        {
            _target = target;
            return this;
        }

        /// <summary>
        /// 设置效果持续时间（-1 表示不自动销毁）
        /// </summary>
        public Task_SpawnEffect SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }

        /// <summary>
        /// 获取生成的效果对象
        /// </summary>
        public GameObject SpawnedEffect => _spawnedEffect;

        protected override async UniTask OnStart()
        {
            if (_effectPrefab == null)
            {
                Debug.LogWarning("Task_SpawnEffect: EffectPrefab is null!");
                EndTask();
                return;
            }

            // 确定生成位置
            Vector3 spawnPos = _target != null ? _target.position : _location;

            // 实例化效果
            _spawnedEffect = GameObject.Instantiate(_effectPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[SpawnEffect] 成功生成: {_spawnedEffect.name}");

            // 如果设置了目标，绑定到目标
            if (_target != null)
            {
                _spawnedEffect.transform.SetParent(_target);
            }

            // 如果设置了持续时间，等待后销毁
            if (_duration > 0)
            {
                Debug.Log($"[SpawnEffect] 等待 {_duration} 秒后销毁");
                await UniTask.Delay((int)(_duration * 1000), cancellationToken: CancellationToken);
                
                if (_spawnedEffect != null)
                {
                    Debug.Log($"[SpawnEffect] 延时到，销毁特效");
                    GameObject.Destroy(_spawnedEffect);
                }
            }

            Debug.Log($"[SpawnEffect] 任务结束");
            EndTask();
        }

        protected override void OnInterrupt()
        {
            // 任务被中断时，销毁生成的效果
            if (_spawnedEffect != null)
            {
                GameObject.Destroy(_spawnedEffect);
                _spawnedEffect = null;
            }
        }
    }
}
