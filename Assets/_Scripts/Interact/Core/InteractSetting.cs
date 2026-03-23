

using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 交互设置 负责配置交互系统相关参数
/// </summary>
[CreateAssetMenu(menuName = "StatSystem/InteractData", fileName = "InteractData")]
public class InteractSetting : ScriptableObject
{
    [Header("射线设置")]
    [SerializeField, LabelText("射线距离")] public float rayDistance = 3f;
    [SerializeField, LabelText("球形检测半径")] public float sphereCastRadius = 0.3f;
    [SerializeField, LabelText("交互层")] public LayerMask interactableLayer;
    [SerializeField, LabelText("检查间隔")] public float checkInterval = 0.1f;
    [SerializeField, LabelText("最大命中数")] public int maxHits = 4;

    [Header("调试")]
    [SerializeField, LabelText("显示检测区域")] public bool showDebug = true;

    [Header("交互键")]
    [SerializeField, LabelText("交互键")] public KeyCode interactKey = KeyCode.E;
}