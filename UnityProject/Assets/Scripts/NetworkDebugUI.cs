using Unity.Netcode;
using UnityEngine;

public class NetworkDebugUI : MonoBehaviour
{
    // OnGUI 是 Unity 老旧但极其方便的即时渲染 UI 系统，特别适合做底层调试
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        // 如果当前既不是主机，也不是客机（还没连上）
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // 渲染三个大按钮
            if (GUILayout.Button("创建主机 (Start Host)", GUILayout.Width(200), GUILayout.Height(50)))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("加入客机 (Start Client)", GUILayout.Width(200), GUILayout.Height(50)))
            {
                NetworkManager.Singleton.StartClient();
            }
            if (GUILayout.Button("专用服务器 (Start Server)", GUILayout.Width(200), GUILayout.Height(50)))
            {
                NetworkManager.Singleton.StartServer();
            }
        }
        else
        {
            // 连上之后，显示当前身份
            string mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            GUILayout.Label($"<size=24><b>当前状态: {mode}</b></size>");
        }

        GUILayout.EndArea();
    }
}