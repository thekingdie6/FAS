using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [Header("UI 预制体")]
    [SerializeField] private GameObject floatingTextPrefab;

    private GameObject currentUIInstance;//当前UI实例对象
    private TextMeshProUGUI textComponent;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if(Instance ==null) Instance = this;
        else Destroy(gameObject);
        currentUIInstance=Instantiate(floatingTextPrefab, transform);
        textComponent=currentUIInstance.GetComponent<TextMeshProUGUI>();
        canvasGroup=currentUIInstance.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        currentUIInstance.SetActive(false);
    } 
    public void ShowTextAtPosition(Vector3 position,string textToShow)
    {
        currentUIInstance.SetActive(true);
        currentUIInstance.transform.position = position + new Vector3(0, 1f,-1f);
        textComponent.text = textToShow;
        canvasGroup.alpha = Mathf.Lerp(0,1,0.1f);
    }

    public void HideText()
    {
        canvasGroup.alpha = 0;
        currentUIInstance.SetActive(false);
    }
}
