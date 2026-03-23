using cfg.Interact;
using UnityEngine;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [Header("物品ID")]
    [SerializeField] private int itemId;

    [Header("测试")]
    [SerializeField] private Color highlightColor = Color.yellow;

    private Color originalColor;
    private ItemInfo _itemInfo;

    private void Start()
    {
        SetItemInfo(itemId);
    }

    public void OnInteractEnter(Vector3 hitPoint)
    {
        
    }

    public void OnInteractStay(Vector3 hitPoint, Vector3 normal)
    {
        
    }

    public void OnInteractExit()
    {
     
    }

    public void Interact()
    {
        
    }

    public void SetItemInfo(int itemId)
    {
        this.itemId = itemId;
        _itemInfo = DataManager.GetItemInfo(itemId);
    }

    public ItemInfo GetItemInfo()
    {
        return _itemInfo;
    }
}
