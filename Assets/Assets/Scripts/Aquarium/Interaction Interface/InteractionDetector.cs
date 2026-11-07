using Assets.Scripts.Aquarium.FishTank;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (CloseCurrentUI())
            return;

        interactableInRange?.Interact();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        CloseCurrentUI();
    }

    private bool CloseCurrentUI()
    {
        if (FishTankUIController.Instance != null && FishTankUIController.Instance.IsOpen)
        {
            FishTankUIController.Instance.Close();
            return true;
        }

        if (WorkbenchUIController.Instance != null && WorkbenchUIController.Instance.IsOpen)
        {
            WorkbenchUIController.Instance.Close();
            return true;
        }

        if (ComputerUIController.Instance != null && ComputerUIController.Instance.IsOpen)
        {
            ComputerUIController.Instance.Close();
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}