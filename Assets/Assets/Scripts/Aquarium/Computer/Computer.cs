using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        Debug.Log("INTERACTION");
        ComputerUIController computerUI = FindFirstObjectByType<ComputerUIController>();
        computerUI.Open();
    }
}
