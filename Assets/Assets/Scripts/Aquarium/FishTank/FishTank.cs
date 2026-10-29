using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Fish in this tank")]
    [SerializeField] private List<Fish> fishes = new List<Fish>();

    public IReadOnlyList<Fish> Fishes => fishes;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        FishTankUIController.Instance.Open(this);
    }

    public bool AddFish(Fish fish)
    {
        fishes.Add(fish);
        Debug.Log(fish.fishName + " added to tank: " + gameObject.name);

        return true;
    }

    public bool RemoveFish(Fish fish)
    {
        bool removed = fishes.Remove(fish);

        if (removed)
            Debug.Log(fish.fishName + " removed from tank: " + gameObject.name);

        return removed;
    }
}
