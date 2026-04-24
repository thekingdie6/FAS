using UnityEngine;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract(GameObject interactor);
}