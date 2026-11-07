using System.Collections.Generic;
using Assets.Scripts.Diving.DivingInventory;
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
            if (GameManager.Instance.Transitioning) return;
            
            if (isDivingScene) // TODO: all of this is stupid
            {
                if (DivingController.Instance.state == PlayerState.PlayerState.GRAB)
                    return;

                List<FishCollectible> fishCollectibles = DivingInventory.Instance.GetCollectiblesOfType<FishCollectible>();
                FishCollectionManager.Instance.AddCollectibles(fishCollectibles);
            }
        
            GameManager.Instance.ChangeScene(targetSceneIndex);
        }
    }
}
