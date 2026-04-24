using UnityEngine;

[System.Flags]
public enum ItemCategory
{
    None = 0,
    Tool=1<<0,
    Weapon=1<<1,
    Crop=1<<2,
    Food=1<<3,
    Material=1<<4
}
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemDataSO : ScriptableObject
{
    [Header("核心数据（core）")]
    [Tooltip("物品唯一标识符，依靠其进行网络同步")]
    public ushort ItemID;
    public string ItemName;
    [TextArea] public string ItemDescription;

    [Header("分类与规则（Categorization&Rules）")]
    public ItemCategory Category;//位掩码枚举，支持多选
    public bool IsStackable => MaxStackSize > 1;//快捷只读属性
    [Min(1)] public int MaxStackSize=1;//默认为1不可堆叠

    [Header("表现层（Visuals）")]
    public Sprite Icon;//背包UI显示的图标

    [Header("物理层（world Entity）")]
    [Tooltip("当玩家从背包丢弃该物品时，在场景中生成的预制体")]
    public GameObject DropPrefab;

    // --- 进阶架构：组件化扩展（供后续开发使用）AI提示 ---
    // 如果一个物品既是食物又是武器，它到底能回多少血？造成多少伤害？
    // 最好的做法是将具体的“作用”剥离成独立的子模块。
    // public ItemAction[] actions;
}
