using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Aquarium.FishTanks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private GameObject interactionIcon;
    
    private readonly Dictionary<IInteractable, Transform> _interactableList = new();

    public readonly UnityEvent OnInteractibleEntered = new();
    public readonly UnityEvent OnInteractibleExited = new();


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (CloseCurrentUI())
            return;

        _interactableList
            .OrderBy(kvp => Vector2.Distance(transform.position, kvp.Value.position))
            .FirstOrDefault()
            .Key?
            .Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (
            !collision.TryGetComponent(out IInteractable interactable) || 
            !interactable.CanInteract() ||
            !_interactableList.TryAdd(interactable, collision.transform)
        ) return;
        
        if (_interactableList.Count == 1) 
        {
            OnInteractibleEntered.Invoke();
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (
            !collision.TryGetComponent(out IInteractable interactable) ||
            !_interactableList.Remove(interactable)
        ) return;

        if (_interactableList.Count == 0) 
        {
            OnInteractibleExited.Invoke();
            interactionIcon.SetActive(false);
        }
    }
    
    private static bool CloseCurrentUI()
    {
        if (FishTankUIController.Instance && FishTankUIController.Instance.IsOpen)
        {
            FishTankUIController.Instance.Close();
            return true;
        }

        if (WorkbenchUIController.Instance && WorkbenchUIController.Instance.IsOpen)
        {
            WorkbenchUIController.Instance.Close();
            return true;
        }

        if (ComputerUIController.Instance && ComputerUIController.Instance.IsOpen)
        {
            ComputerUIController.Instance.Close();
            return true;
        }

        return false;
    }
}