using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItemDataBaseSO", menuName = "Inventory/ItemDataBaseSO")]
public class ItemDataBaseSO : ScriptableObject
{
    [Tooltip("全游戏物品配置")]
    public List<ItemDataSO> AllItems;
}
