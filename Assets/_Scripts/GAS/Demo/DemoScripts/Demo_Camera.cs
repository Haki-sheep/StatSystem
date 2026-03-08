using UnityEngine;

/// <summary>
/// 俯视跟随相机 - 类似LOL/王者荣耀视角
/// </summary>
public class Demo_Camera : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;

    [Header("相机设置")]
    [SerializeField] private float height = 15f;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("角度设置")]
    [SerializeField, Range(30, 80)] private float pitchAngle = 45f;

    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 mapBounds = new Vector2(50, 50);

    private Vector3 currentPosition;
    private Quaternion currentRotation;

    private void Start()
    {
        // 初始化相机位置
        if (target != null)
        {
            UpdateCameraPosition();
        }

        // 计算初始角度
        currentRotation = Quaternion.Euler(pitchAngle, 0, 0);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        UpdateCameraPosition();
    }

    /// <summary>
    /// 更新相机位置
    /// </summary>
    private void UpdateCameraPosition()
    {
        // 计算目标位置（相机应该在目标后上方）
        Vector3 targetCamPos = target.position - Vector3.forward * distance + Vector3.up * height;

        // 平滑跟随
        currentPosition = Vector3.Lerp(currentPosition, targetCamPos, followSpeed * Time.deltaTime);

        // 应用边界限制
        if (useBounds)
        {
            currentPosition.x = Mathf.Clamp(currentPosition.x, -mapBounds.x, mapBounds.x);
            currentPosition.z = Mathf.Clamp(currentPosition.z, -mapBounds.y, mapBounds.y);
        }

        transform.position = currentPosition;
        transform.rotation = Quaternion.Euler(pitchAngle, 0, 0);
    }

    /// <summary>
    /// 设置跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 调整相机高度（滚轮缩放）
    /// </summary>
    public void AdjustHeight(float delta)
    {
        height = Mathf.Clamp(height + delta, 5f, 30f);
        distance = Mathf.Clamp(distance + delta, 5f, 25f);
    }

    /// <summary>
    /// 调整俯角
    /// </summary>
    public void AdjustPitch(float delta)
    {
        pitchAngle = Mathf.Clamp(pitchAngle + delta, 30f, 80f);
    }
}
