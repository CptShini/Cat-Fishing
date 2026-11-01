using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ExitArea : MonoBehaviour, IInteractable
    {
        [SerializeField] private int targetSceneIndex;
        [SerializeField] private bool isDivingScene;

        public bool CanInteract() => true;

        public void Interact()
        {
            if (isDivingScene) // TODO: all of this is stupid
            {
                if (DivingController.Instance.state == PlayerState.PlayerState.GRAB)
                    return;
            
                FishCollectionManager.Instance.TransferFishCollectibles(DivingInventory.Instance.GetCollectibleofType<FishCollectible>());
            }
        
            SceneTransition.Instance.LoadScene(targetSceneIndex);
        }
    }
}
