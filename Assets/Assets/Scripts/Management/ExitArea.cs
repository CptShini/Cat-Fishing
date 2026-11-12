using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Management
{
    public class ExitArea : MonoBehaviour, IInteractable
    {
        [SerializeField] private Scenes targetScene;
        [SerializeField] private AudioId triggerAudio;
        [SerializeField] private UnityEvent onExit;
        [SerializeField] private string exitingInfo = "...";
        public bool CanInteract() => true;

        public void Interact()
        {
            if (GameManager.Instance.Transitioning) return;
            
            onExit.Invoke();
            AudioManager.Instance.Play(triggerAudio);
            GameManager.Instance.ChangeScene(targetScene, exitingInfo);
        }
    }
}
