using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.Targeting;
using GAS.TaskSystem;
using UnityEngine;

namespace GAS.Core
{
    /// <summary>
    /// 等待目标数据任务 - 等待玩家选择目标
    /// </summary>
    public class Task_WaitTargetData : AbilityTask
    {
        /// <summary>
        /// 采集到的目标数据
        /// </summary>
        public TargetData TargetData { get; private set; }

        /// <summary>
        /// 是否取消等待
        /// </summary>
        private bool _cancelRequested;

        /// <summary>
        /// 创建等待目标数据任务
        /// </summary>
        public static Task_WaitTargetData WaitTargetData(AbilitySystemComponent owner)
        {
            var task = new Task_WaitTargetData();
            task.Init(owner);
            return task;
        }

        /// <summary>
        /// 请求取消等待（可从外部调用）
        /// </summary>
        public void RequestCancel()
        {
            _cancelRequested = true;
        }

        protected override async UniTask OnStart()
        {
            // TODO: 这里需要接入你的输入系统/UI系统来获取目标选择
            // 
            // 方案 1: 射线检测（简单实现）
            // 方案 2: 等待 UI 回调
            // 方案 3: 等待输入系统
            //
            // 这里提供一个简单的射线检测实现作为示例

            TargetData = new TargetData();

            while (!_cancelRequested && !CancellationToken.IsCancellationRequested)
            {
                // 检测鼠标点击
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // 检测是否点击了有效目标
                        if (hit.collider.TryGetComponent(out AbilitySystemComponent targetASC))
                        {
                            TargetData.TargetList.Add(targetASC);
                            TargetData.TargetLocation = hit.point;
                            break;
                        }
                        else
                        {
                            // 点击了地面，记录位置
                            TargetData.TargetLocation = hit.point;
                            break;
                        }
                    }
                }

                // 每帧检测一次
                await UniTask.Yield();
            }

            EndTask();
        }

        protected override void OnInterrupt()
        {
            _cancelRequested = true;
            TargetData = null;
        }
    }
}
