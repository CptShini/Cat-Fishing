using UnityEngine;

public class ExitArea : MonoBehaviour, IInteractable
{
    [SerializeField] private string scene = "AquariumScene";

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        SceneTransition.Instance.LoadScene(scene);
    }
}
