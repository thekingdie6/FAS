using UnityEngine;
using Unity.Netcode;
using System.Collections; // 必须引入这个命名空间才能使用协程

// 强制要求挂载AudioSource组件
[RequireComponent(typeof(AudioSource))]
public class CropItemInteract : NetworkBehaviour, IInteractable
{
    [SerializeField] private CropController cropControllerScript;

    [Header("音效设置")]
    public AudioClip waterSound;       // 拖入你那段较长的浇水音效
    public float maxSoundDuration = 1f;// 限制音效的最大播放时长（秒）
    private AudioSource audioSource;

    private void Awake()
    {
        // 获取当前作物身上的AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GlobalTimeManager.OnDayChanged += GrowCrop;
    }
    private void OnDisable()
    {
        GlobalTimeManager.OnDayChanged -= GrowCrop;
    }

    private void GrowCrop(int currentDay)
    {
        cropControllerScript.spriteRenderer.sprite = cropControllerScript.growthSprites[cropControllerScript.nutrient.Value];
        if (!IsServer) return;
        cropControllerScript.isWatered.Value = false;
        Debug.Log($"今日是第{currentDay}日，{gameObject.name}生长开始，浇水状态{cropControllerScript.isWatered.Value}");
    }

    public string GetInteractPrompt()
    {
        return "采摘完毕！";
    }

    // ================== 核心交互逻辑 ==================
    public void OnInteract(GameObject interactor)
    {
        if (!IsServer) return; // 只有服务器有权进行以下判断

        if (cropControllerScript.growthStage.Value == CropData.GrowthStage.ripeness)
        {
            Server_HarvestCrop();
            Debug.Log($"作物被{interactor.name}捡起了！");
            return;
        }

        if (cropControllerScript.isWatered.Value == true)
        {
            Debug.Log("今日已浇过水");
            return; // 已经浇过水，直接返回，不触发音效
        }

        // --- 走到这里，说明【浇水成功】 ---
        cropControllerScript.isWatered.Value = true;
        cropControllerScript.nutrient.Value += 1;
        Debug.Log($"浇水成功，养分+1...");

        // 1. 调用阶段检查
        StageChange(cropControllerScript.nutrient.Value);

        // 2. 通知所有客户端（包括浇水的人和旁边的人）：播放浇水音效！
        PlayWaterSoundClientRpc();
    }

    // ================== 音效与时长控制 ==================
    [ClientRpc]
    private void PlayWaterSoundClientRpc()
    {
        if (waterSound != null && audioSource != null)
        {
            // 给音调加一点点随机波动，让每次浇水听起来都不太一样
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            // 设置音频并播放
            audioSource.clip = waterSound;
            audioSource.Play();

            // 开启协程，强制在指定时间后掐断声音
            StartCoroutine(StopSoundAfterTime(maxSoundDuration));
        }
    }

    // 协程：等待指定秒数后停止播放
    private IEnumerator StopSoundAfterTime(float duration)
    {
        // 暂停运行指定的秒数（比如 1 秒）
        yield return new WaitForSeconds(duration);

        // 时间到了，如果声音还在播，就强制停止
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void StageChange(int nutrient)
    {
        if (nutrient >= cropControllerScript.data.nutrientNeed)
        {
            cropControllerScript.growthStage.Value = CropData.GrowthStage.ripeness;
            Debug.Log($"养分充足，现在有{cropControllerScript.nutrient.Value}点养分，成熟了！");
        }
    }

    public void Server_HarvestCrop()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        ItemStack loot = new ItemStack { ItemID = cropControllerScript.data.ItemID, Amount = 3 };

        GameObject dropGo = Instantiate(cropControllerScript.data.DropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        ItemEntity entityScript = dropGo.GetComponent<ItemEntity>();
        dropGo.GetComponent<NetworkObject>().Spawn();
        entityScript.Payload.Value = loot;
    }
}