using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryManager : MonoBehaviour
{
    public static UI_InventoryManager Instance { get;private set; }

    [Header("UI 预制体与容器设置")]
    [Tooltip("单格预制体（UI_Slots）")]
    public UI_Slot SlotPrefab;

    [Tooltip("快捷栏父节点（Horizontal Layout Group）")]
    public Transform HotbarParent;

    [Tooltip("大背包格子父节点（Grid Layout Group）")]
    public Transform BackpackParent;

    [Tooltip("大背包整体面板（控制tab键显隐）")]
    public GameObject BackpackPanel;

    [Header("背包配置")]
    public int HotbarSize = 9;//快捷栏数量
    public int BackpackSize = 27;//背包额外数量

    private UI_Slot[] _allUISlots;//内部名单，记录格子预制体外壳
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if(Instance!=null&&Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if(BackpackPanel!=null) { BackpackPanel.SetActive(false); }
        //初始生成所有格子
        InitializeSlots();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleBackpack();
        }
    }

    private void InitializeSlots()
    {
        int totalCapacity = HotbarSize + BackpackSize;
        _allUISlots = new UI_Slot[totalCapacity];

        for(int i = 0; i< totalCapacity; i++)
        {
            Transform targetParent = (i < HotbarSize) ? HotbarParent : BackpackParent;
            UI_Slot newSlot=Instantiate(SlotPrefab, targetParent);
            newSlot.Initialize(i);
            _allUISlots[i]= newSlot;
        }
    }
    private void ToggleBackpack()
    {
        if(BackpackPanel!= null)
        {
            bool isActive=BackpackPanel.activeSelf;
            BackpackPanel.SetActive(!isActive);
        }
    }

    private void RefreshSlotUI(int index,ushort itemID,int amount)
    {
        if (index < 0 || index >= _allUISlots.Length) return;
        ItemDataSO data = ItemManager.Instance.GetItemData(itemID);
        _allUISlots[index].UpdateView(data,amount);
    }

    public void OnSlotClicked(int slotIndex)
    {
        Debug.Log($"玩家点击了{slotIndex}个格子！");
    }
}
