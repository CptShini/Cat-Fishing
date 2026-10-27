using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        WorkbenchUIController workbenchUI = FindFirstObjectByType<WorkbenchUIController>();
        workbenchUI.Open();
    }
}
