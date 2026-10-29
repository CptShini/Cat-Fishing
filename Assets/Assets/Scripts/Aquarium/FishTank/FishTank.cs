using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Tank settings")]
    [SerializeField] private string tankId;
    [SerializeField] private FishDatabase fishDatabase;

    [Header("Fish in this tank")]
    [SerializeField] private List<Fish> fishes = new List<Fish>();

    public string TankId => tankId;
    public IReadOnlyList<Fish> Fishes => fishes;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        FishTankUIController.Instance.Open(this);
    }

    private void Start()
    {
        fishes.Clear();
        fishes.AddRange(FishSaveSystem.LoadTank(tankId, fishDatabase));
    }

    public bool AddFish(Fish fish)
    {
        if (fish == null || fishes.Exists(f => f != null && f.fishId == fish.fishId))
            return false;
        
        fishes.Add(fish);
        FishSaveSystem.SaveTank(this);
        Debug.Log(fish.fishName + " added to tank: " + gameObject.name);

        return true;
    }

    public bool RemoveFish(Fish fish)
    {
        int index = fishes.FindIndex(f => f != null && f.fishId == fish.fishId);

        if (index < 0)
            return false;

        Fish removedFish = fishes[index];
        fishes.RemoveAt(index);

        FishSaveSystem.SaveTank(this);

        Debug.Log(removedFish.fishName + " removed from tank: " + gameObject.name);

        return true;
    }
}
