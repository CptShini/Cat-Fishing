using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Tank settings")]
    [SerializeField] private string tankId;
    [SerializeField] private FishDatabase fishDatabase;

    [Header("Fish in this tank")]
    [SerializeField] private List<FishDTO> fishes = new List<FishDTO>();

    public string TankId => tankId;
    public IReadOnlyList<FishDTO> Fishes => fishes;

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

    public bool AddFish(FishDTO fishDto)
    {
        if (fishDto == null || fishes.Exists(f => f != null && f.Id == fishDto.Id))
            return false;
        
        fishes.Add(fishDto);
        FishSaveSystem.SaveTank(this);
        Debug.Log(fishDto.Name + " added to tank: " + gameObject.name);

        return true;
    }

    public bool RemoveFish(FishDTO fishDto)
    {
        int index = fishes.FindIndex(f => f != null && f.Id == fishDto.Id);

        if (index < 0)
            return false;

        FishDTO removedFishDto = fishes[index];
        fishes.RemoveAt(index);

        FishSaveSystem.SaveTank(this);

        Debug.Log(removedFishDto.Name + " removed from tank: " + gameObject.name);

        return true;
    }

    public void ClearTank()
    {
        fishes.Clear();
        FishSaveSystem.SaveTank(this);
    }
}
