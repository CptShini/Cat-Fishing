using Assets.Scripts.Audio.SFX;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Management
{
    public class ExitArea : MonoBehaviour, IInteractable
    {
        [SerializeField] private Scenes targetScene;
        [SerializeField] private SFXId triggerSFX;
        [SerializeField] private UnityEvent onExit;
        [SerializeField] private string exitingInfo = "...";
        public bool CanInteract() => true;

        public void Interact()
        {
            if (GameManager.Instance.Transitioning) return;
            
            onExit.Invoke();
            SFXManager.Instance.Play(triggerSFX);
            GameManager.Instance.ChangeScene(targetScene, exitingInfo);
        }
    }
}
