using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("角色预制体")]
    public GameObject playerPrefab;

    [Header("Relay UI 组件")]
    public TMP_InputField joinCodeInput; // 放在主菜单供客机输入
    public TextMeshProUGUI mainMenuJoinCodeText; // 放在主菜单供主机显示（虽然跳转很快，但保留用于调试）

    // 【核心：接力棒】静态变量，跨场景不会消失
    public static string CurrentJoinCode = "";

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"✅ 匿名登录成功！玩家ID: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ 初始化或登录失败: {e}");
        }
    }

    // ================== 主机逻辑区 ==================
    public async void StartGame()
    {
        try
        {
            Debug.Log("正在向 Relay 申请创建房间...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 【关键点】存入静态变量，带往下一个场景
            CurrentJoinCode = joinCode;

            Debug.Log($"房间创建成功！联机代码: {joinCode}");
            if (mainMenuJoinCodeText != null) mainMenuJoinCodeText.text = $"房间号: {joinCode}";

            RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

   
            // 跳转到游戏场景
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay 创建失败: {e}");
        }
    }

    // ================== 客机逻辑区 ==================
    public async void JoinGame()
    {
        Debug.Log("==== 加入游戏按钮被物理按下了！====");

        if (joinCodeInput == null)
        {
            Debug.LogError("🚨 输入框对象神秘消失了！");
            return;
        }
        string joinCode = joinCodeInput.text.Trim();
        if (string.IsNullOrEmpty(joinCode)) return;

        try
        {
            // 客机也将自己输入的代码存入静态变量，方便在自己的屏幕上显示
            CurrentJoinCode = joinCode;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay 加入失败: {e}");
        }
    }

    // ================== 角色生成逻辑（保持不变） ==================
    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "SampleScene")
        {
            foreach (ulong clientId in clientsCompleted)
            {
                GameObject playerInstance = Instantiate(playerPrefab);
                playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    public void ExitGame() { Application.Quit(); }
}