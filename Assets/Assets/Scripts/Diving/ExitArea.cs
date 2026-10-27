using UnityEngine;

public class ExitArea : MonoBehaviour, IInteractable
{


    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        SceneTransition.Instance.LoadScene("AquariumScene");
    }

}
