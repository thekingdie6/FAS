using UnityEngine;
using Unity.Netcode;
public class ItemEntity : NetworkBehaviour
{
    [Header("网络数据承载")]
    public NetworkVariable<ItemStack> Payload = new NetworkVariable<ItemStack>(new ItemStack { ItemID=0,Amount=0},NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    [Header("吸附参数设置")]
    public float MagnetRadius = 3.0f;
    public float MagnetSpeed = 10.0f;
    public float PickupDistance = 0.5f;

    [Header("视觉层")]
    public SpriteRenderer SpriteRenderer;

    private bool _canBeMagnetized = false;
    private Transform _targetPlayer = null;

    public override void OnNetworkSpawn()
    {
        RefreshVisuals();
        Payload.OnValueChanged += (oldVal, newVal) => RefreshVisuals();

        if(IsServer)
        {
            Invoke(nameof(EnableMagnet), 1.0f);
        }
    }
    private void RefreshVisuals()
    {
        if (Payload.Value.ItemID == 0) return;
        ItemDataSO data=ItemManager.Instance.GetItemData(Payload.Value.ItemID);
        if (data != null) SpriteRenderer.sprite = data.Icon;
        Debug.Log("图片已切换！");
    }
    private void EnableMagnet()
    {
        _canBeMagnetized = true;
    }
    private void Update()
    {
        if (!IsServer || !_canBeMagnetized) return;
        if(_targetPlayer==null)
        {
            FindClosestPlayer();//如果目标玩家为空，查询最近的玩家
        }
        else
        {
            FlyTowardsPlayer();
        }
    }
    private void FindClosestPlayer()
    {
        Collider2D[] coliders=Physics2D.OverlapCircleAll(transform.position,MagnetRadius);
        foreach(var col in  coliders)
        {
            if(col.CompareTag("Player"))
            {
                _targetPlayer = col.transform;
                break;//结束，锁定当前目标
            }
        }
    }
    private void FlyTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, MagnetSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, _targetPlayer.position)<=PickupDistance)
        {
            PickupItem(_targetPlayer.gameObject);
        }
    }
    private void PickupItem(GameObject player)
    {
        InventoryNetWorkManager inventory=player.GetComponent<InventoryNetWorkManager>();
        if(inventory!=null)
        {
            bool success = inventory.Server_AddLoot(Payload.Value);
            if(success)
            {
                GetComponent<NetworkObject>().Despawn();
                Debug.Log("拾取成功！");
            }
            else
            {
                _targetPlayer = null;
                _canBeMagnetized= false;
                Invoke(nameof(EnableMagnet), 2.0f);
            }
                    }
    }
}
