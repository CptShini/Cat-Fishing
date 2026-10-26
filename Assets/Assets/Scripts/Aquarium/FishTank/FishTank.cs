using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Fish in this tank")]
    [SerializeField] private List<FishSO> fishes = new List<FishSO>();

    public IReadOnlyList<FishSO> Fishes => fishes;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        FishTankUIController.Instance.Open(this);
    }
}
