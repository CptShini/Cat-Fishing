using Assets.Scripts.Audio;
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
        AudioManager.Instance.Play(AudioId.ButtonClick);
        computerUI.Open();
    }
}