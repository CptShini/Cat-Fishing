using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Fish in this tank")]
    [SerializeField] private List<FishSO> fishes = new List<FishSO>();

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
        
        Debug.Log("Fish in this tank:");
        foreach (FishSO fish in fishes)
        {
            Debug.Log("- " + fish.fishName);
        }
    }
}
