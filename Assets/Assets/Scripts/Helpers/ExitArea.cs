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
            if (isDivingScene)
            {
                if (DivingController.Instance.state == Assets.Scripts.PlayerState.PlayerState.GRAB)
                    return;
            
                FishCollectionManager.Instance.TransferFishCollectibles(DivingInventory.Instance.GetCollectibleofType<FishCollectible>());
            }
        
            SceneTransition.Instance.LoadScene(targetSceneIndex);
        }
    }
}
