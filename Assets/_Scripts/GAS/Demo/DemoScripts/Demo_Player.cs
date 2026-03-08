using GAS.Component;
using GAS.Core;
using GAS.Core.GameplayEffect;
using GAS.StateSystem;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Demo_Player : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("组件")]
    [SerializeField] private CharacterController characterController;

    [Header("GAS组件")]
    [SerializeField] private AbilitySystemComponent abilitySystemComponent;
    [SerializeField] private GEManager geManager;
    [SerializeField] private StatController statController;

    [Header("目标选择")]
    [SerializeField] private Transform currentTarget;

    private Vector3 moveDirection;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        // 自动获取GAS组件
        abilitySystemComponent ??= GetComponent<AbilitySystemComponent>();
        geManager ??= GetComponent<GEManager>();
        statController ??= GetComponent<StatController>();
    }

    private void Start()
    {
        // 初始化StatController
        statController?.Init();
        Debug.Log($"[Demo_Player] Start: statController={(statController != null)}, IsInit={statController?.IsInit}");
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// 处理移动输入
    /// </summary>
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            moveDirection = direction * moveSpeed;
        }
        else
        {
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, rotationSpeed * Time.deltaTime);
        }

        // 应用重力
        moveDirection.y = -9.8f;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// 处理角色朝向（鼠标控制）
    /// </summary>
    private void HandleRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// 获取当前移动方向（用于技能释放）
    /// </summary>
    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    /// <summary>
    /// 获取角色朝向方向
    /// </summary>
    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

    /// <summary>
    /// 获取角色位置
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    #region GAS接口

    /// <summary>
    /// 尝试激活技能
    /// </summary>
    public bool TryActivateAbility(string abilityName)
    {
        Debug.Log($"[Player] TryActivateAbility called - abilityName: {abilityName}");
        
        if (abilitySystemComponent == null)
        {
            Debug.LogWarning("[Player] abilitySystemComponent is null!");
            return false;
        }
        
        if (statController == null)
        {
            Debug.LogWarning("[Player] statController is null!");
            return false;
        }

        Debug.Log($"[Player] 调用 AbilitySystemComponent.TryActivateAbility");
        return abilitySystemComponent.TryActivateAbility(abilityName, statController);
    }

    /// <summary>
    /// 应用GE到自身
    /// </summary>
    public void ApplyEffectToSelf(GameplayEffectData effectData)
    {
        abilitySystemComponent?.ApplyGE(effectData, this);
    }

    /// <summary>
    /// 获取AbilitySystemComponent
    /// </summary>
    public AbilitySystemComponent GetAbilitySystemComponent()
    {
        return abilitySystemComponent;
    }

    /// <summary>
    /// 获取StatController
    /// </summary>
    public StatController GetStatController()
    {
        return statController;
    }

    /// <summary>
    /// 获取GE管理器
    /// </summary>
    public GEManager GetGEManager()
    {
        return geManager;
    }

    #endregion

    #region 目标选择

    [Header("自动选敌设置")]
    [SerializeField] private float autoTargetRange = 30f;
    [SerializeField] private LayerMask enemyLayerForAutoTarget;

    /// <summary>
    /// 获取当前目标（如果没有手动选中目标，自动选择最近敌人）
    /// </summary>
    public Transform GetCurrentTarget()
    {
        // 如果已经有手动选择的目标，直接返回
        if (currentTarget != null)
            return currentTarget;

        // 自动选择最近敌人
        return FindNearestEnemy();
    }

    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    private Transform FindNearestEnemy()
    {
        // 如果没有设置特定的Layer，则搜索所有带Collider的物体
        Collider[] colliders;
        if (enemyLayerForAutoTarget.value != 0)
        {
            colliders = Physics.OverlapSphere(transform.position, autoTargetRange, enemyLayerForAutoTarget);
        }
        else
        {
            // 默认搜索所有碰撞体（排除自己）
            colliders = Physics.OverlapSphere(transform.position, autoTargetRange);
        }
        
        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            // 排除自己
            if (collider.transform == transform)
                continue;
                
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        if (nearestEnemy != null)
        {
            Debug.Log($"[自动选敌] 已自动锁定最近敌人: {nearestEnemy.name}");
        }

        return nearestEnemy;
    }

    /// <summary>
    /// 设置当前目标（用于雷电球）
    /// </summary>
    public void SetCurrentTarget(Transform target)
    {
        currentTarget = target;
    }

    /// <summary>
    /// 清除目标
    /// </summary>
    public void ClearTarget()
    {
        currentTarget = null;
    }

    #endregion
}
