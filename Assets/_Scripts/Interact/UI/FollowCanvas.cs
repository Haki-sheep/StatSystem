using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using cfg.Interact;
using DG.Tweening;

[RequireComponent(typeof(Canvas))]
public class FollowCanvas : MonoBehaviour
{
    [Header("组件设置")]
    [SerializeField, LabelText("相机")] private Transform cameraTransform;
    [SerializeField, LabelText("CanvasGroup")] private CanvasGroup canvasGroup;


    [Header("表现设置")]
    [SerializeField, LabelText("旋转速度")] private float rotationSpeed = 10f;
    [SerializeField, LabelText("偏移")] private Vector3 offset = new Vector3(0, 0.3f, 0);
    [SerializeField, LabelText("淡入时长")] private float fadeInDuration = 0.3f;
    [SerializeField, LabelText("淡出时长")] private float fadeOutDuration = 0.3f;

    [SerializeField, LabelText("高亮颜色")] private Color highGridColor;
    [SerializeField, LabelText("默认颜色")] private Color lowGridColor;

    [Header("UIPanel")]
    [SerializeField, LabelText("Icon")] private Image icon;
    [SerializeField, LabelText("文本")] private TextMeshProUGUI textComponent;
    [SerializeField, LabelText("网格组父物体")] private GameObject gridPrefab;

    private Image[] gridArray = new Image[16];
    private Transform target; // 跟随目标
    private ItemInfo itemInfo; // 物品数据
    private int gridIndex = 0; // 网格索引
    private Tween fadeTween; // 当前淡入淡出Tween

    private void Start()
    {
        // 设置CanvasGroup避免拦截射线
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);

        for (int i = 0; i < gridArray.Length; i++)
        {
            gridArray[i] = gridPrefab.transform.GetChild(i).GetComponent<Image>();
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            SetVisable(false);
            return;
        }

        UpdatePosition();
        UpdateRotation();
    }


    /// <summary>
    /// 设置显隐
    /// </summary>
    /// <param name="show"></param>
    public void SetVisable(bool show)
    {
        // 停止之前的Tween
        fadeTween?.Kill();

        if (show)
        {
            // 显示面板
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            // 设置CanvasGroup不拦截射线
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
            }

            // DoTween淡入
            fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, fadeInDuration)
                .SetUpdate(UpdateType.Late);
        }
        else
        {
            // DoTween淡出
            if (canvasGroup != null)
            {
                fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, fadeOutDuration)
                    .SetUpdate(UpdateType.Late)
                    .OnComplete(() =>
                    {
                        if (gameObject.activeSelf)
                            gameObject.SetActive(false);
                    });
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    private void UpdatePosition()
    {
        transform.position = target.position + offset;
    }

    /// <summary>
    /// 更新旋转
    /// </summary>
    private void UpdateRotation()
    {
        if (cameraTransform == null)
            return;

        Vector3 lookDirection = cameraTransform.position - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// 设置相机
    /// </summary>
    /// <param name="camTransform"></param>
    public void SetCamera(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="target"></param>
    /// <param name="itemInfo"></param>
    public void SetTargetInfo(Transform target, ItemInfo itemInfo)
    {
        SetDefaultColor();

        this.target = target;
        this.itemInfo = itemInfo;

        SetItemInfoToUI();
    }

    /// <summary>
    /// 清除目标
    /// </summary>
    public void Clear()
    {
        target = null;
        itemInfo = null;
    }

    private void SetItemInfoToUI()
    {
        if (itemInfo == null)
            return;

        // 通过 Resources 加载图标
        if (!string.IsNullOrEmpty(itemInfo.ItemIcon))
        {
            var sprite = Resources.Load<Sprite>(itemInfo.ItemIcon);
            icon.sprite = sprite;
        }

        textComponent.text = itemInfo.ItemName + "\n" + itemInfo.ItemDescription;

        SetGridVisable();
    }

    /// <summary>
    /// 根据Item信息更新网格高亮
    /// </summary>
    private void SetGridVisable()
    {
        if (itemInfo == null)
            return;

        int itemWidth = itemInfo.Weight;
        int itemHeight = itemInfo.Height;
        int colCount = 4;    // 4列

        // 遍历物品占用的格子，设置高亮
        for (int h = 0; h < itemHeight; h++)
        {
            for (int w = 0; w < itemWidth; w++)
            {
                // 计算网格索引 : 行 * 列 + 列
                int gridIndex = h * colCount + w;

                // 边界检查
                if (gridIndex < gridArray.Length)
                {
                    gridArray[gridIndex].color = Color.white;  // 高亮
                }
            }
        }
    }

    /// <summary>
    /// 设置默认网格颜色
    /// </summary>
    private void SetDefaultColor()
    {
        foreach (var grid in gridArray)
        {
            grid.color = lowGridColor;
        }
    }
}
