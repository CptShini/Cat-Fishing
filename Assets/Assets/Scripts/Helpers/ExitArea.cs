using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Helpers
{
    public class ExitArea : MonoBehaviour, IInteractable
    {
        [SerializeField] private int targetSceneIndex;
        [SerializeField] private AudioId triggerAudio;
        [SerializeField] private UnityEvent onExit;

        public bool CanInteract() => true;

        public void Interact()
        {
            if (GameManager.Instance.Transitioning) return;
            
            onExit.Invoke();
            AudioManager.Instance.Play(triggerAudio);
            GameManager.Instance.ChangeScene(targetSceneIndex);
        }
    }
}
