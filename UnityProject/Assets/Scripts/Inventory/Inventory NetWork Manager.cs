using UnityEngine;
using Unity.Netcode;

public class InventoryNetWorkManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("背包配置")]
    public int MaxSlots = 36;

    public NetworkList<ItemStack> Inventory=   new NetworkList<ItemStack>(null,NetworkVariableReadPermission.Owner,NetworkVariableWritePermission.Server);
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
            Inventory[0] = new ItemStack { ItemID = 1, Amount = 1 };
            Inventory[1] = new ItemStack { ItemID = 2, Amount = 2 };
        }
        else
        {
            LoadPlayerDataFromDatabase(OwnerClientId);
        }
    }
    private void HandleInventoryChanged(NetworkListEvent<ItemStack> changeEvent)
    {

    }
    private void ForceRefreshUI()
    {

    }
    private bool CheckIfNewPlayer(ulong cliented) { return true;/*待时现*/ }
    private void LoadPlayerDataFromDatabase(ulong clientID) { /*待实现*/}
}
