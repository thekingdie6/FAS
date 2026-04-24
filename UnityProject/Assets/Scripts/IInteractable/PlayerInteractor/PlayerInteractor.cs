using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class PlayerInteractor : NetworkBehaviour
{
    public Grid gameGrid;
    public override void OnNetworkSpawn()
    {
       gameGrid=Object.FindFirstObjectByType<Grid>();
        //gameGrid=MapManager.Instance.MainGrid;预写，没验证
    }
    public float interactRange = 1f;
     void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("按下空格！");
            TryInteract();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void TryInteract()
    {
        Vector2 origin = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//获取鼠标的连续世界坐标
        Vector2 dirction = ((Vector2)mouseWorldPos - origin).normalized;
        RaycastHit2D hit = Physics2D.Raycast(origin, dirction, interactRange, LayerMask.GetMask("NPC"));
        Debug.DrawRay(origin, dirction * interactRange, Color.yellow,2.0f);
        Debug.Log("发射射线！");
        if (hit.collider != null)
        {
            Debug.Log($"射线检测到了目标！NPC：{hit.collider.name}");
            if(hit.collider.TryGetComponent<IInteractable>(out var crop))
            {
                ulong targetId = hit.collider.GetComponent<NetworkObject>().NetworkObjectId;
                RequestInteractServerRpc(targetId);
            }
            return;
        }
        Vector3Int cellPos = gameGrid.WorldToCell(mouseWorldPos);
        Vector3 cellCenter = gameGrid.GetCellCenterWorld(cellPos);
        if((Vector2.Distance(transform.position, cellCenter)<=interactRange) )
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(cellCenter, LayerMask.GetMask("Crops"));
            if(hitCollider != null)
            {
                Debug.Log($"网格检测到目标!{hitCollider.name}");
                if(hitCollider.TryGetComponent<IInteractable>(out var crop))
                {
                    ulong targetId=hitCollider.GetComponent<NetworkObject>().NetworkObjectId;
                    RequestInteractServerRpc(targetId);
                }
            }
        }
    }
    [ServerRpc]
    void RequestInteractServerRpc(ulong targetNetworkObjectId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId,out NetworkObject targetObj))
        {
            targetObj.GetComponent<IInteractable>().OnInteract(gameObject);
            //targetObj.Despawn();
        }
    }
}
