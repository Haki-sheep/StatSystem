using Cysharp.Threading.Tasks;
using GAS.Component;
using GAS.Core;
using UnityEngine;

namespace GAS.TaskSystem
{
    public class TaskSystemTest : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private AbilitySystemComponent _playerASC;
        [SerializeField] private GameObject _testEffectPrefab;
        [SerializeField] private KeyCode _testWaitKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode _testSpawnKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode _testTargetKey = KeyCode.Alpha3;

        private void Start()
        {
            Debug.Log("=== Task 系统测试 ===");
            Debug.Log("按键 1: 测试 Task_Wait (等待1秒)");
            Debug.Log("按键 2: 测试 Task_SpawnEffect (生成特效2秒后销毁)");
            Debug.Log("按键 3: 测试 Task_WaitTargetData (点击鼠标选择目标)");

            // 自动获取 ASC
            if (_playerASC == null)
                _playerASC = GetComponent<AbilitySystemComponent>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_testWaitKey))
            {
                TestWait();
            }

            if (Input.GetKeyDown(_testSpawnKey))
            {
                TestSpawnEffect();
            }

            if (Input.GetKeyDown(_testTargetKey))
            {
                TestWaitTargetData();
            }
        }

        /// <summary>
        /// 测试 Task_Wait - 等待1秒
        /// </summary>
        private async void TestWait()
        {
            Debug.Log($"[{Time.time:F2}] 开始测试 Task_Wait...");

            var task = Task_Wait.Wait(_playerASC, 1f);
            task.Start();

            Debug.Log($"[{Time.time:F2}] Task_Wait 已启动，等待1秒...");

            await UniTask.WaitUntil(() => task.IsEnded);

            Debug.Log($"[{Time.time:F2}] Task_Wait 完成!");
        }

        /// <summary>
        /// 测试 Task_SpawnEffect - 生成特效
        /// </summary>
        private void TestSpawnEffect()
        {
            Debug.Log($"[{Time.time:F2}] 开始测试 Task_SpawnEffect...");

            if (_testEffectPrefab == null)
            {
                Debug.LogWarning("请在 Inspector 中设置 Test Effect Prefab!");
                return;
            }

            var task = Task_SpawnEffect.SpawnEffect(_playerASC)
                .SetEffectPrefab(_testEffectPrefab)
                .SetLocation(_playerASC.transform.position + Vector3.forward * 2)
                .SetDuration(2f);

            task.Start();

            Debug.Log($"[{Time.time:F2}] Task_SpawnEffect 已启动，特效将持续2秒");
        }

        /// <summary>
        /// 测试 Task_WaitTargetData - 目标选择
        /// </summary>
        private async void TestWaitTargetData()
        {
            Debug.Log($"[{Time.time:F2}] 开始测试 Task_WaitTargetData...");
            Debug.Log("请在场景中点击一个目标 (或有 Collider 的物体)");

            var task = Task_WaitTargetData.WaitTargetData(_playerASC);
            task.Start();

            // 等待用户选择目标
            await UniTask.WaitUntil(() => task.IsEnded);

            if (task.TargetData != null && task.TargetData.IsValid)
            {
                Debug.Log($"[{Time.time:F2}] 目标选择完成!");
                Debug.Log($"  - 目标数量: {task.TargetData.TargetList.Count}");
                Debug.Log($"  - 目标位置: {task.TargetData.TargetLocation}");

                if (task.TargetData.SingleTarget != null)
                {
                    Debug.Log($"  - 选中目标: {task.TargetData.SingleTarget.name}");
                }
            }
            else
            {
                Debug.Log($"[{Time.time:F2}] 目标选择取消或无效");
            }
        }

        /// <summary>
        /// 绘制测试提示 UI
        /// </summary>
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUILayout.Label("=== Task 系统测试 ===", style);
            GUILayout.Label("按键 1: 测试 Task_Wait", style);
            GUILayout.Label("按键 2: 测试 Task_SpawnEffect", style);
            GUILayout.Label("按键 3: 测试 Task_WaitTargetData", style);

            if (Application.isPlaying)
            {
                GUILayout.Label($"当前时间: {Time.time:F2}", style);
            }
        }
    }
}