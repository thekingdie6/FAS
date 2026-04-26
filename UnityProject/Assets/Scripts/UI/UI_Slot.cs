using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_Slot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image Icon;
    public TextMeshProUGUI AmountText;

    public int SlotIndex {  get; private set; }

    public void Initialize(int index)
    {
        SlotIndex = index;
        GetComponent<Button>().onClick.AddListener(() => UI_InventoryManager.Instance.OnSlotClicked(SlotIndex));
    }
    public void UpdateView(ItemDataSO data,int amount)
    {
        if(data==null||amount==0)
        {
            Icon.gameObject.SetActive(false);
            AmountText.text = "";
        }
        else
        {
            Icon.gameObject.SetActive(true);
            Icon.sprite = data.Icon;
            AmountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
}
