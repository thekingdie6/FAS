using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_Slots : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image Icon;
    public TextMeshProUGUI AmountText;

    public int SlotIndex {  get; private set; }

    public void Initialize(int index)
    {
        SlotIndex = index;
        //GetComponent<Button>().onClick.AddListener(()=>UIInventoryManager.Instance.on)
    }
}
