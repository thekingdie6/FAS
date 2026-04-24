using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static ItemManager Instance {  get; private set; }

    [Header("配置数据")]
    public ItemDataBaseSO DatabaseConfig;

    private Dictionary<ushort, ItemDataSO> _itemDictionary;
    private void Awake()
    {
        if(Instance !=null&& Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);//过场景不销毁
        //2.构建字典（核心操作）
        
    }
    private void InitializeDatabase()
    {
        _itemDictionary =new Dictionary<ushort, ItemDataSO>();
        if(DatabaseConfig==null||DatabaseConfig.AllItems==null)
        {
            Debug.LogError("物品注册表未配置！");
            return;
        }
        foreach(var item in DatabaseConfig.AllItems)
        {
            if (item == null) continue;
            if(_itemDictionary.ContainsKey(item.ItemID))
            {
                Debug.LogError($"发现重复的物品 ID：{item.ItemID}({item.ItemName}),请检查配置！");
                continue;
            }
        }
        Debug.Log($"物品注册表加载完毕，共加载了{_itemDictionary.Count}种物品。");
    }
    public ItemDataSO GetItemData(ushort itemID)
    {
        if(_itemDictionary.TryGetValue(itemID,out ItemDataSO data))
        {
            return data;
        }
        Debug.LogWarning($"未找到ID为{itemID}的物品！");
        return null;
    }

    public bool IsValidItem(ushort itemID)
    {
        return _itemDictionary.ContainsKey(itemID);
    }
}
