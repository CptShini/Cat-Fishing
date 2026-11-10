using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Managers
{
    [RequireComponent(typeof(DiscoveryManager))]
    public class NotificationsManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [SerializeField] private Image notificationImage;
        [SerializeField] private TextMeshProUGUI text;

        [SerializeField] private float speed = 1;
        [SerializeField] private float duration = 3;
    
        private float curDuration;
        private bool started;
    
        private void Start()
        {
            DiscoveryManager dm = GetComponent<DiscoveryManager>();
            dm.Discovered.AddListener(OnDiscovery);
            dm.NewRecord.AddListener(OnRecord);
        }

        private void OnDiscovery(Fish fish)
        {
            text.text = "New Fish Discovered!";
            notificationImage.sprite = fish.Species.Icon;
            Notify();
        }

        private void OnRecord(Fish fish)
        {
            text.text = "New Record!";
            notificationImage.sprite = fish.Species.Icon;
            Notify();
        }

        private void Notify()
        {
            animator.SetFloat("speed", speed);
            animator.SetBool("finished", false);
            animator.Play("NotificationUp");
            curDuration = 0;
            started = true;
        }

        private void StopNotify()
        {
            animator.SetBool("finished", true);
            started = false;
        }
    
        private void Update()
        {
            switch (started)
            {
                case false:
                    return;
                case true when curDuration > duration:
                    StopNotify();
                    break;
                default:
                    curDuration += Time.deltaTime;
                    break;
            }
        }
    }
}
