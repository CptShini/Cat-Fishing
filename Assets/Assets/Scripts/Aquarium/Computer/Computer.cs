using Assets.Scripts.Audio.SFX;
using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        ComputerUIController computerUI = FindFirstObjectByType<ComputerUIController>();
        SFXManager.Instance.Play(SFXId.ButtonClick);
        computerUI.Open();
    }
}