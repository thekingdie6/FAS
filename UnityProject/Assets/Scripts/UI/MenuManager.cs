using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public GameObject playerPrefab;

    // ================== 主机逻辑区 ==================
    public void StartGame()
    {
        NetworkManager.Singleton.StartHost();

        // 1. 监听场景加载完成（为主机自己生成角色）
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;

        // 2. 【新增】监听新客机连入（为后续加入的客机生成角色）
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "SampleScene")
        {
            Debug.Log("游戏场景已加载完毕！主机准备生成初始角色...");
            foreach (ulong clientId in clientsCompleted)
            {
                GameObject playerInstance = Instantiate(playerPrefab);
                playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
        }
    }

    // 【新增】专门处理客机加入的方法（这段代码只有主机有权限执行）
    private void OnClientConnected(ulong clientId)
    {
        // 安全校验：排除主机自己。因为主机(LocalClientId)已经在上面的 OnSceneLoaded 里生成过了
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"检测到客机 {clientId} 加入！主机正在为其生成专属角色...");

            // 主机代劳：实例化并分配给这个新来的客机
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    // ================== 客机逻辑区 ==================
    public void JoinGame()
    {
        Debug.Log("客机尝试连接服务器...");
        // 客机只需要这一句话！连上之后，Netcode 会自动让客机加载场景，
        // 然后触发主机的 OnClientConnected，主机就会帮客机生成角色。
        NetworkManager.Singleton.StartClient();
    }

    // ================== 通用逻辑区 ==================
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Exit");
    }
}