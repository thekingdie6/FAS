using UnityEngine;
using Unity.Netcode;
using Unity.Collections; // 【新增】用于引入网络定长字符串
using TMPro;
public class PlayerAppearanceSync : NetworkBehaviour
{
    [Header("视觉层级引用 (纸娃娃部件)")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer headRenderer;
    // public SpriteRenderer weaponRenderer; // 预留

    [Header("UI 引用")]
    public TextMeshProUGUI nameText; // 【新增】用于挂载我们刚才创建的 NameText

    [Header("美术资源库 (前期测试用)")]
    // 前期测试：这里放猫、狗的“整体图片”
    public Sprite[] bodySkins;
    // 后期：public Sprite[] headSkins;

    // 【核心解耦】使用 NetworkVariable 同步各部位 ID
    // WritePermission.Owner 允许客机自己决定自己的长相，并自动同步给全网！
    public NetworkVariable<int> netBodyID = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    // 【新增】同步玩家名字的网络变量。
    // 注意：Netcode 不允许直接同步 string，必须用 FixedString，这里用 32 字节（足够装十几个汉字或字母了）
    public NetworkVariable<FixedString32Bytes> netPlayerName = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> netHeadID = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        // 1. 如果是本地玩家，把大厅选择的形象 ID 写入网络变量
        if (IsOwner)
        {
            netBodyID.Value = LocalPlayerCache.BodyID;
            netHeadID.Value = LocalPlayerCache.HeadID;
            // 将 string 转换为 FixedString32Bytes 存入网络
            netPlayerName.Value = new FixedString32Bytes(LocalPlayerCache.PlayerName);
        }

        // 2. 无论主机客机，生成时立刻刷新一次长相和名字
        UpdateBodyVisual(0, netBodyID.Value);
        UpdateNameVisual("", netPlayerName.Value);
        // 3. 绑定监听：以后只要 ID 变了，全网自动换装！
        netBodyID.OnValueChanged += UpdateBodyVisual;
        netPlayerName.OnValueChanged += UpdateNameVisual; // 【新增】名字监听
        // netHeadID.OnValueChanged += UpdateHeadVisual; // 预留给后期
    }

    public override void OnNetworkDespawn()
    {
        // 养成好习惯，销毁时注销事件防止内存泄漏
        netBodyID.OnValueChanged -= UpdateBodyVisual;
        netPlayerName.OnValueChanged -= UpdateNameVisual;
    }

    // 真正的换装逻辑
    private void UpdateBodyVisual(int oldID, int newID)
    {
        // 安全校验
        if (bodySkins == null || bodySkins.Length == 0) return;

        // 防止越界
        int safeIndex = Mathf.Clamp(newID, 0, bodySkins.Length - 1);

        // 替换精灵图
        if (bodyRenderer != null)
        {
            bodyRenderer.sprite = bodySkins[safeIndex];
        }
    }
    private void UpdateNameVisual(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        if (nameText != null)
        {
            // 将网络专用的 FixedString 转换回 C# 普通 string 并显示在 UI 上
            nameText.text = newName.ToString();
        }
    }
}