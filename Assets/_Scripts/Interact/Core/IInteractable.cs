using cfg.Interact;
using UnityEngine;

/// <summary>
/// 交互接口 负责实现交互逻辑
/// </summary> 
public interface IInteractable
{  
    /// <summary>
    /// 获取物品数据
    /// </summary>
    /// <returns></returns>
    ItemInfo GetItemInfo();

    /// <summary>
    /// 设置物品数据
    /// </summary>
    void SetItemInfo(int itemId);

    /// <summary>
    /// 进入射线检测
    /// </summary>
    /// <param name="hitPoint"></param>
    void OnInteractEnter(Vector3 hitPoint);

    /// <summary>
    /// 持续检测
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <param name="normal"></param>
    void OnInteractStay(Vector3 hitPoint, Vector3 normal);

    /// <summary>
    /// 退出射线检测
    /// </summary>
    void OnInteractExit();

    /// <summary>
    /// 交互逻辑
    /// </summary>
    void Interact();
}
