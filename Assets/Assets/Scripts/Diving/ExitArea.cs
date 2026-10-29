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
            Debug.Log("TRANSFERING");
            FishCollectionManager.Instance.TransferFishCollectibles(DivingInventory.Instance.GetCollectibleofType<FishCollectible>());
        }
        SceneTransition.Instance.LoadScene(scene);
    }
}
