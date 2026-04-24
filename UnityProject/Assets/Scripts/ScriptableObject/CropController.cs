using UnityEngine;
using Unity.Netcode;
public class CropController : NetworkBehaviour
{
    [Header("数据来源")]
    public CropData data;

    [Header("本地视觉配置")]
    public Sprite[] growthSprites;
    public SpriteRenderer spriteRenderer;

    [Header("对象网络状态")]
    public NetworkVariable<int> currentGrowthDay=new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isWatered=new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<CropData.GrowthStage> growthStage = new NetworkVariable<CropData.GrowthStage>(CropData.GrowthStage.seed, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<int> nutrient = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void OnNetworkSpawn()
    {
        spriteRenderer.sprite= growthSprites[0];
        Debug.Log($"种下一颗{data.cropName},作物周期是{data.growthDate}天");
    }

}
