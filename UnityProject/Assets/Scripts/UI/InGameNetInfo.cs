using UnityEngine;
using TMPro;

public class InGameNetInfo : MonoBehaviour
{
    public TextMeshProUGUI displayResultText;

    void Start()
    {
        // 从 MenuManager 的“接力棒”里读取房间号
        if (!string.IsNullOrEmpty(MenuManager.CurrentJoinCode))
        {
            displayResultText.text = "房间代码: " + MenuManager.CurrentJoinCode;
        }
        else
        {
            displayResultText.text = "本地模式/未连接";
        }
    }
}