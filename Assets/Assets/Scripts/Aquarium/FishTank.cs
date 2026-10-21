using UnityEngine;

public class FishTank : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public bool CanInteract()
    {
        return true; // will change it later probably to check when to open or close fishtank UI
    }

    public void Interact()
    {
        if(!CanInteract())
        {
            Debug.Log("Cannot interact with the: " + gameObject.name);
            return;
        }
        Debug.Log("Interacting with the: " + gameObject.name);
    }
}
