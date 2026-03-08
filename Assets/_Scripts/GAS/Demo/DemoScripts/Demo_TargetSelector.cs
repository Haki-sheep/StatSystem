using UnityEngine;

public class Demo_TargetSelector : MonoBehaviour
{
    [Header("玩家引用")]
    [SerializeField] private Demo_Player player;

    [Header("设置")]
    [SerializeField] private float maxTargetDistance = 20f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Color selectedColor = Color.red;
    [SerializeField] private Color hoverColor = Color.yellow;

    private Transform currentTarget;
    private Transform hoverTarget;
    private Renderer currentRenderer;
    private Renderer hoverRenderer;
    private Color originalColor;

    private void Update()
    {
        HandleTargetSelection();
        UpdateTargetHighlight();
    }

    private void HandleTargetSelection()
    {
        // 鼠标左键点击选择目标
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxTargetDistance, enemyLayer))
            {
                var newTarget = hit.collider.transform;
                if (newTarget != currentTarget)
                {
                    SetTarget(newTarget);
                }
            }
            else
            {
                ClearTarget();
            }
        }
    }

    private void UpdateTargetHighlight()
    {
        // 检测鼠标悬停
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxTargetDistance, enemyLayer))
        {
            var newHover = hit.collider.transform;
            if (newHover != hoverTarget)
            {
                ClearHoverHighlight();
                hoverTarget = newHover;
                hoverRenderer = hoverTarget.GetComponent<Renderer>();
                if (hoverRenderer != null && hoverTarget != currentTarget)
                {
                    hoverRenderer.material.color = hoverColor;
                }
            }
        }
        else
        {
            ClearHoverHighlight();
        }
    }

    private void SetTarget(Transform target)
    {
        ClearCurrentHighlight();
        currentTarget = target;
        currentRenderer = currentTarget.GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            originalColor = currentRenderer.material.color;
            currentRenderer.material.color = selectedColor;
        }

        // 设置到玩家
        player?.SetCurrentTarget(currentTarget);

        Debug.Log($"[目标] 选中目标: {currentTarget.name}");
    }

    private void ClearTarget()
    {
        ClearCurrentHighlight();
        currentTarget = null;
        player?.ClearTarget();
        Debug.Log("[目标] 取消选中目标");
    }

    private void ClearCurrentHighlight()
    {
        if (currentRenderer != null && currentTarget != null)
        {
            currentRenderer.material.color = originalColor;
            currentRenderer = null;
        }
    }

    private void ClearHoverHighlight()
    {
        if (hoverRenderer != null && hoverTarget != currentTarget)
        {
            hoverRenderer.material.color = originalColor;
            hoverRenderer = null;
        }
        hoverTarget = null;
    }
}
