using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class PlayerCameraFollow : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        Debug.Log("我是主人！");
        var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
        if( vcam != null )
        {
            Debug.Log("获取成功！");
            vcam.Follow = this.transform;
        }
    }
}
