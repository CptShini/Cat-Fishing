using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Fish.Domain;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Tank settings")]
    [SerializeField] private string tankId;
    [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

    [Header("Fish in this tank")]
    [SerializeField] private List<Fish> fishes = new();

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
        fishes.AddRange(FishSaveSystem.LoadTank(tankId, fishSpeciesDatabase));
    }

    public bool AddFish(Fish fish)
    {
        if (fish == null || fishes.Exists(f => f != null && f.Id == fish.Id))
            return false;
        
        fishes.Add(fish);
        FishSaveSystem.SaveTank(this);
        Debug.Log(fish.Species.Name + " added to tank: " + gameObject.name);

        return true;
    }

    public bool RemoveFish(Fish fish)
    {
        int index = fishes.FindIndex(f => f != null && f.Id == fish.Id);

        if (index < 0)
            return false;

        Fish removedFish = fishes[index];
        fishes.RemoveAt(index);

        FishSaveSystem.SaveTank(this);

        Debug.Log(removedFish.Species.Name + " removed from tank: " + gameObject.name);

        return true;
    }

    public void ClearTank()
    {
        fishes.Clear();
        FishSaveSystem.SaveTank(this);
    }
}
