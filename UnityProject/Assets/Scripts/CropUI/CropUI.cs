using UnityEngine;
using Unity.Netcode;

//[RequireComponent(typeof(CanvasGroup))]
public class CropUI : MonoBehaviour
{
    [Header("data source")]
    [SerializeField] private CropData cropData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isOwner = other.GetComponent<Unity.Netcode.NetworkObject>().IsLocalPlayer;
        if(isOwner)
        {
            UIManager.Instance.ShowTextAtPosition(transform.position, cropData.cropName);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        bool isOwner = other.GetComponent<Unity.Netcode.NetworkObject>().IsLocalPlayer;

        if (isOwner)
        {
            UIManager.Instance.HideText();
        }
    }
    private void OnDestroy()
    {
        if (UIManager.Instance!=null)
        {
            UIManager.Instance.HideText();
        }
    }
    // Update is called once per frame
}
