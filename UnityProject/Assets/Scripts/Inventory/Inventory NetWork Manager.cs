using UnityEngine;
using Unity.Netcode;

public class InventoryNetWorkManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("背包配置")]
    public int MaxSlots = 36;

    public NetworkList<ItemStack> Inventory=  new NetworkList<ItemStack>(null,NetworkVariableReadPermission.Owner,NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemStack> MouseSlot;//一个用来置换的空间
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            InitializeInventoryData();
        }
        if(IsOwner)
        {
            Inventory.OnListChanged += HandleInventoryChanged;

            ForceRefreshUI();
        }
    }
    public override void OnNetworkDespawn()
    {
        if(IsOwner)
        {
            Inventory.OnListChanged -= HandleInventoryChanged;
        }
    }
    private void InitializeInventoryData()
    {
        for(int i=0;i<MaxSlots;i++)
        {
            Inventory.Add(new ItemStack { ItemID = 0, Amount = 0 });
        }
        bool isNewPlayer=CheckIfNewPlayer(OwnerClientId);
        if (isNewPlayer)
        {
            Inventory[0] = new ItemStack { ItemID = 001, Amount = 1 };
            Inventory[1] = new ItemStack { ItemID = 002, Amount = 2 };
        }
        else
        {
            LoadPlayerDataFromDatabase(OwnerClientId);
        }
    }
    //背包列表发生变化时，网络底层会自动触发这个方法
    private void HandleInventoryChanged(NetworkListEvent<ItemStack> changeEvent)
    {
        if (UI_InventoryManager.Instance == null) return;

        UI_InventoryManager.Instance.RefreshSlotUI(
            changeEvent.Index, //发生变动的格子序号
            changeEvent.Value.ItemID, 
            changeEvent.Value.Amount//最新的ItemStack数据
            );
    }

    //玩家刚连入房间或者刚打开UI，强制对齐一次所有数据
    private void ForceRefreshUI()
    {
        if (UI_InventoryManager.Instance == null) return;
        for(int i=0;i<Inventory.Count; i++)
        {
            UI_InventoryManager.Instance.RefreshSlotUI(i, Inventory[i].ItemID, Inventory[i].Amount);
        }
    }
    private bool CheckIfNewPlayer(ulong cliented) { return true;/*待实现*/ }
    private void LoadPlayerDataFromDatabase(ulong clientID) { /*待实现*/}
    public void RequestMoveItem(int fromIndex, int toIndex)
    {
        if(IsOwner)
        {
            MoveItemServerRpc(fromIndex, toIndex);
        }
    }

    public bool Server_AddLoot(ItemStack newLoot)
    {
        if (!IsServer || newLoot.IsEmpty) return false;
        int maxStack=ItemManager.Instance.GetItemData(newLoot.ItemID).MaxStackSize;
        for(int i=0; i<MaxSlots;i++)
        {
            if (Inventory[i].ItemID == newLoot.ItemID && Inventory[i].Amount<maxStack)
            {
                int spaceLeft = maxStack - Inventory[i].Amount;
                int amountToAdd=Mathf.Min(newLoot.Amount,spaceLeft);
                Inventory[i] = new ItemStack { ItemID = Inventory[i].ItemID, Amount = Inventory[i].Amount + amountToAdd };
                newLoot.Amount -= amountToAdd;
                if (newLoot.Amount <= 0) return true;
            }
        }
        for (int i = 0; i < MaxSlots; i++)
        {
            if (Inventory[i].ItemID == 0)
            {
                int amountToAdd = Mathf.Min(newLoot.Amount, maxStack);
                Inventory[i] = new ItemStack { ItemID = newLoot.ItemID, Amount = amountToAdd };
                newLoot.Amount -= amountToAdd;
                if (newLoot.Amount <= 0) return true;
            }
        }
            return newLoot.Amount <= 0;
    }

    [ServerRpc]
    private void MoveItemServerRpc(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= MaxSlots || toIndex < 0 || toIndex >= MaxSlots) return;
        ItemStack fromItem=Inventory[fromIndex];
        ItemStack toItem=Inventory[toIndex];
        if (fromItem.IsEmpty) return;
        if(fromItem.ItemID== toItem.ItemID&&!toItem.IsEmpty)
        {
            int maxStack = ItemManager.Instance.GetItemData(toItem.ItemID).MaxStackSize;
            int totalAmount = fromItem.Amount + toItem.Amount;
            if (totalAmount <= maxStack)
            {
                Inventory[toIndex]=new ItemStack { ItemID=toItem.ItemID,Amount=totalAmount };
                Inventory[fromIndex]=new ItemStack {ItemID=0,Amount=0};
            }
            else
            {
                Inventory[toIndex] = new ItemStack { ItemID = toItem.ItemID, Amount = maxStack };
                Inventory[fromIndex] = new ItemStack { ItemID = fromItem.ItemID, Amount = totalAmount - maxStack };
            }
        }
        else 
        {
                Inventory[toIndex] = fromItem;
                Inventory[fromIndex] = toItem;
        }
    }
}
