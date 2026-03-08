using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// 等待任务 - 等待指定时间
    /// </summary>
    public class Task_Wait : AbilityTask
    {
        private float _duration;

        /// <summary>
        /// 创建等待任务
        /// </summary>
        public static Task_Wait Wait(AbilitySystemComponent owner, float duration)
        {
            var task = new Task_Wait();
            task.Init(owner);
            task._duration = duration;
            return task;
        }

        public Task_Wait SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }

        protected override async UniTask OnStart()
        {
            await UniTask.Delay((int)(_duration * 1000), cancellationToken: CancellationToken);
            EndTask();
        }
    }
}
