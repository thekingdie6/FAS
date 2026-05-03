using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UI_Slot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image Icon;
    public TextMeshProUGUI AmountText;

    private ItemDataSO _currentItemData;//当前格子存储的是什么

    public int SlotIndex {  get; private set; }

    public void Initialize(int index)
    {
        SlotIndex = index;
        GetComponent<Button>().onClick.AddListener(() => UI_InventoryManager.Instance.OnSlotClicked(SlotIndex));
    }
    public void UpdateView(ItemDataSO data,int amount)
    {
        _currentItemData = data;
        if(data==null||amount==0)
        {
            Icon.gameObject.SetActive(false);
            AmountText.text = "";
        }
        else
        {
            Icon.gameObject.SetActive(true);
            Icon.sprite = data.Icon;
            AmountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentItemData == null) return;//空的拖不了
        Icon.color = new Color(1, 1, 1, 0.5f);//半透明设置
        UI_InventoryManager.Instance.StartDrag(_currentItemData);
        if (!UI_InventoryManager.Instance.CanInterect()) return;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(_currentItemData == null) return;//理所当然，空的拖不了
        UI_InventoryManager.Instance.UpdateDragPosition(eventData.position);//获取鼠标坐标的位置传给背包管理器，让幽灵图跟着鼠标坐标走
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //if (_currentItemData == null) return;
        Icon.color = new Color(1, 1, 1, 1f);//回复透明度
        UI_InventoryManager.Instance.EndDrag();
    }
    public void OnDrop(PointerEventData eventData)
    {
        UI_Slot sourceSlot=eventData.pointerDrag.GetComponent<UI_Slot>();//获取鼠标拖拽事件带过来的对象上有没有UI_Slot,是不是预制体

        if(sourceSlot != null &&sourceSlot!=this)//是预制体并且不是这个预制体
        {
            Debug.Log($"玩家把{sourceSlot.SlotIndex}格的物品，拖到了{this.SlotIndex}格！");
            var localPlayerObject = Unity.Netcode.NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();//获取本地玩家网络对象;
            if(localPlayerObject != null &&localPlayerObject.TryGetComponent<InventoryNetWorkManager>(out var inventory))//尝试获取本地玩家身上的背包管理器
            {
                inventory.RequestMoveItem(sourceSlot.SlotIndex, this.SlotIndex);//将移动请求发送给服务器！
            }
        }
    }
}
