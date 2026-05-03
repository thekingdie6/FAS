using UnityEngine;
using Unity.Netcode;
public class CropItemInteract : NetworkBehaviour,IInteractable
{
    [SerializeField] private CropController cropControllerScript;
    private void OnEnable()
    {
        GlobalTimeManager.OnDayChanged += GrowCrop;
    }
    private void OnDisable()
    {
        GlobalTimeManager.OnDayChanged -= GrowCrop;
    }
    //[SerializeField] private CropData cropData;
    private void GrowCrop(int currentDay)
    {
        cropControllerScript.spriteRenderer.sprite = cropControllerScript.growthSprites[cropControllerScript.nutrient.Value];
        if (!IsServer) return;
        cropControllerScript.isWatered.Value = false;
        Debug.Log("状态切换报错！");
        Debug.Log($"今日是第{currentDay}日，{gameObject.name}生长开始，浇水状态{cropControllerScript.isWatered.Value}");
    }
    public string GetInteractPrompt()
    {
        return "采摘完毕！";
    }
    public void OnInteract(GameObject interactor)
    {
        if (!IsServer) return;
        if(cropControllerScript.growthStage.Value==CropData.GrowthStage.ripeness)//作物是不是成熟阶段
        {
            Server_HarvestCrop();
            //Destroy(gameObject);
            Debug.Log($"作物被{interactor.name}捡起了！");
            return;
        }
        if (cropControllerScript.isWatered.Value == true)//有没有浇过水
        {
            Debug.Log("今日已浇过水");
            return;
        }
        cropControllerScript.isWatered .Value= true;//没浇过水那就浇水
        cropControllerScript.nutrient.Value += 1;//养分加一
        Debug.Log($"浇水成功，养分+1，现阶段为{ cropControllerScript.growthStage.Value}，养份为：{cropControllerScript.nutrient.Value}，浇水状态：{cropControllerScript.isWatered.Value}");
        StageChange(cropControllerScript.nutrient.Value);//调用阶段检查
    }
    public void StageChange(int nutrient)//获取养分值
    {
        if(nutrient>=cropControllerScript.data.nutrientNeed)//养份比需要的总养分多或者一样
        {
            cropControllerScript.growthStage.Value = CropData.GrowthStage.ripeness;//切换为成熟阶段
            Debug.Log($"养分充足，现在有{cropControllerScript.nutrient.Value}点养分，成熟了！");
        }
    }
    public void Server_HarvestCrop()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        ItemStack loot = new ItemStack { ItemID =cropControllerScript.data.ItemID,Amount=3};
        Debug.Log("获取了对象ID");
        GameObject dropGo = Instantiate(cropControllerScript.data.DropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        ItemEntity entityScript=dropGo.GetComponent<ItemEntity>();
        dropGo.GetComponent<NetworkObject>().Spawn();
        entityScript.Payload.Value = loot;
    }

}
