using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Fish in this tank")]
    [SerializeField] private List<FishSO> fishes = new List<FishSO>();

    public IReadOnlyList<FishSO> Fishes => fishes;

    public bool CanInteract()
    {
        return true; // will change it later probably to check when to open or close fishtank UI
    }

    public void Interact()
    {
        FishTankUI.Instance.Open(this);
    }
}
