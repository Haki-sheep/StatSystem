using System;
using cfg;
using cfg.Interact;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// 数据管理器 负责初始化Luban数据表和提供数据访问
/// </summary>
public class DataManager : MonoBehaviour
{
    public static Tables Tables { get; private set; } // 所有表格的管理器

    private void Awake()
    {
        InitializeTables();
    }

    /// <summary>
    /// 初始化数据表
    /// </summary>
    private void InitializeTables()
    {
        Func<string, JArray> loader = (filename) =>
        {
            // 文件位于 Resources/ExcelToJson/ 目录下
            TextAsset textAsset = Resources.Load<TextAsset>("ExcelToJson/" + filename);
            if (textAsset == null)
            {
                Debug.LogError($"[DataManager] 找不到数据文件: ExcelToJson/{filename}");
                return null;
            }
            return JArray.Parse(textAsset.text);
        };

        Tables = new Tables(loader);
        Debug.Log("[DataManager] 数据表初始化完成");
    }

    /// <summary>
    /// 根据ID获取物品信息
    /// </summary>
    /// <param name="id">物品ID</param>
    /// <returns>物品信息</returns>
    public static ItemInfo GetItemInfo(int id)
    {
        if (Tables == null)
        {
            Debug.LogError("[DataManager] Tables未初始化");
            return null;
        }

        return Tables.TbItemInfo.GetOrDefault(id);
    }

    /// <summary>
    /// 根据ID获取物品图标
    /// </summary>
    /// <param name="id">物品ID</param>
    /// <returns>物品图标Sprite</returns>
    public static Sprite GetItemIcon(int id)
    {
        ItemInfo itemInfo = GetItemInfo(id);
        if (itemInfo == null || string.IsNullOrEmpty(itemInfo.ItemIcon))
            return null;

        return Resources.Load<Sprite>(itemInfo.ItemIcon);
    }
}
