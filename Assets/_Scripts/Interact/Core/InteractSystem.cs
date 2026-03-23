using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 交互系统 负责更新射线检测 与 (玩家按键)交互处理
/// </summary>
public class InteractSystem : MonoBehaviour
{
    [Header("配置数据")]
    [SerializeField, LabelText("交互配置")] private InteractSetting interactData;

    [Header("引用")]
    [SerializeField, LabelText("玩家相机")] private Camera playerCamera;
    [SerializeField, LabelText("跟随Canvas")] private FollowCanvas followCanvas;

    private RaycastHit[] raycastHits;
    private float timer;
    private Transform currentTarget;
    private IInteractable currentInteractable;
    private Vector3 hitPoint;

    public Transform CurrentTarget => currentTarget;
    public IInteractable CurrentInteractable => currentInteractable;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (followCanvas != null)
            followCanvas.SetCamera(playerCamera.transform);

        int maxHits = interactData != null ? interactData.maxHits : 4;
        raycastHits = new RaycastHit[maxHits];
    }

    private void Update()
    {
        if (interactData == null)
            return;

        // 射线检测
        timer += Time.deltaTime;
        if (timer >= interactData.checkInterval)
        {
            timer = 0f;
            PerformRaycast();
        }

        // 交互处理
        if (currentInteractable != null && Input.GetKeyDown(interactData.interactKey))
            currentInteractable.Interact();

        // 信息更新
        if (currentTarget != null && followCanvas != null)
        {
            followCanvas.SetTargetInfo(currentTarget, currentInteractable.GetItemInfo());
        }
    }

    /// <summary>
    /// 执行射线检测，检测前方是否有可交互物体
    /// </summary>
    private void PerformRaycast()
    {
        if (playerCamera == null)
            return;

        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        // 使用SphereCast进行范围检测
        int hitCount = PhysicRayCast.SphereCastNonAlloc(
            rayOrigin,
            interactData.sphereCastRadius,
            rayDirection,
            raycastHits,
            interactData.rayDistance,
            interactData.interactableLayer,
            QueryTriggerInteraction.Collide,
            true);

        if (hitCount > 0)
        {
            // 从多个命中中选择屏幕中心最近的那个
            RaycastHit bestHit = default;
            float minScreenDistance = float.MaxValue;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = raycastHits[i];
                Vector3 hitPoint = hit.point;
                Vector3 screenPoint = playerCamera.WorldToScreenPoint(hitPoint);

                if (screenPoint.z > 0) // 在相机前方
                {
                    float screenDist = Vector2.Distance(new Vector2(screenPoint.x, screenPoint.y), screenCenter);
                    if (screenDist < minScreenDistance)
                    {
                        minScreenDistance = screenDist;
                        bestHit = hit;
                    }
                }
            }

            if (minScreenDistance < float.MaxValue)
            {
                ProcessHit(bestHit);
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    /// <summary>
    /// 处理命中结果
    /// </summary>
    private void ProcessHit(RaycastHit hitInfo)
    {
        Transform hitTransform = hitInfo.collider.transform;
        hitPoint = hitInfo.point;

        // 检测到新目标
        if (currentTarget != hitTransform)
        {
            // 触发上一个目标的离开事件
            if (currentInteractable != null)
                currentInteractable.OnInteractExit();

            // 设置新目标
            currentTarget = hitTransform;
            currentInteractable = hitTransform.GetComponent<IInteractable>();

            // 如果新目标实现了IInteractable，触发进入事件
            if (currentInteractable != null)
            {
                currentInteractable.OnInteractEnter(hitPoint);
                followCanvas.SetTargetInfo(currentTarget, currentInteractable.GetItemInfo());
                followCanvas.SetVisable(true);
            }
            else
            {
                followCanvas.SetVisable(false);
            }
        }
        else
        {
            // 持续检测同一目标，触发Stay事件
            currentInteractable?.OnInteractStay(hitPoint, hitInfo.normal);
        }
    }

    /// <summary>
    /// 清除当前目标
    /// </summary>
    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentInteractable?.OnInteractExit();
            currentTarget = null;
            currentInteractable = null;
            if (followCanvas != null)
            {
                followCanvas.Clear();
                followCanvas.SetVisable(false);
            }
        }
    }

}
