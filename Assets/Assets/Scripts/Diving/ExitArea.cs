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
        if(scene == "Aquarium") // Assume this is in diving scene
        {
            if(DivingController.Instance.state == Assets.Scripts.PlayerState.PlayerState.GRAB)
            {
                return;
            }
            
            Debug.Log("TRANSFERING");
            FishCollectionManager.Instance.TransferFishCollectibles(DivingInventory.Instance.GetCollectibleofType<FishCollectible>());
        }
        SceneTransition.Instance.LoadScene(scene);
    }
}
