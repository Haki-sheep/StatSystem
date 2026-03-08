using System.Threading;
using Cysharp.Threading.Tasks;
using GAS.Component;

namespace GAS.TaskSystem
{
    /// <summary>
    /// 技能任务基类 - 基于 UniTask
    /// </summary>
    public abstract class AbilityTask
    {
        /// <summary>
        /// 任务拥有者
        /// </summary>
        protected AbilitySystemComponent Owner { get; private set; }

        /// <summary>
        /// 任务是否正在运行
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// 任务是否已结束
        /// </summary>
        public bool IsEnded { get; protected set; }

        /// <summary>
        /// CancellationToken
        /// </summary>
        public CancellationToken CancellationToken => Owner?.GetCancellationTokenOnDestroy() ?? default;

        /// <summary>
        /// 初始化任务
        /// </summary>
        public virtual void Init(AbilitySystemComponent owner)
        {
            Owner = owner;
            IsRunning = false;
            IsEnded = false;
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public virtual void Start()
        {
            IsRunning = true;
            OnStart().Forget();
        }

        /// <summary>
        /// 任务主体逻辑
        /// </summary>
        protected abstract UniTask OnStart();

        /// <summary>
        /// 结束任务
        /// </summary>
        protected virtual void EndTask()
        {
            IsRunning = false;
            IsEnded = true;
            OnEnd();
        }

        /// <summary>
        /// 任务结束回调（可重写）
        /// </summary>
        protected virtual void OnEnd() { }

        /// <summary>
        /// 中断任务
        /// </summary>
        public virtual void InterruptTask()
        {
            if (!IsEnded)
            {
                IsRunning = false;
                IsEnded = true;
                OnInterrupt();
            }
        }

        /// <summary>
        /// 任务被中断回调（可重写）
        /// </summary>
        protected virtual void OnInterrupt() { }
    }
}
