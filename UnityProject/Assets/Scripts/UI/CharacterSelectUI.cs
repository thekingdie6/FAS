using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("UI 组件")]
    public TMP_InputField NameInputField;
    public Image PreviewImage;
    public Button Btn_NextSkin;
    public Button Btn_PrevSkin;

    [Header("形象配置")]
    // 这里前期先放入几张不同的头图精灵测试，比如猫和狗的头部贴图
    public Sprite[] AvailableHeadSkins;

    private int _currentSkinIndex = 0;

    private void Start()
    {
        // 初始化监听
        Btn_NextSkin.onClick.AddListener(NextSkin);
        Btn_PrevSkin.onClick.AddListener(PrevSkin);
        NameInputField.onValueChanged.AddListener(OnNameChanged);

        // 刷新初始 UI
        UpdateVisualPreview();
    }

    private void NextSkin()
    {
        _currentSkinIndex++;
        if (_currentSkinIndex >= AvailableHeadSkins.Length) _currentSkinIndex = 0;
        UpdateVisualPreview();
    }

    private void PrevSkin()
    {
        _currentSkinIndex--;
        if (_currentSkinIndex < 0) _currentSkinIndex = AvailableHeadSkins.Length - 1;
        UpdateVisualPreview();
    }

    private void UpdateVisualPreview()
    {
        if (AvailableHeadSkins.Length > 0)
        {
            PreviewImage.sprite = AvailableHeadSkins[_currentSkinIndex];
        }

        // 【核心】将当前选择的 ID 存入静态接力棒中
        LocalPlayerCache.BodyID = _currentSkinIndex;
    }

    private void OnNameChanged(string newName)
    {
        // 【核心】将输入的名字存入静态接力棒中
        // 如果玩家输入为空，则给一个默认名
        LocalPlayerCache.PlayerName = string.IsNullOrEmpty(newName) ? "神秘玩家" : newName;
    }
}