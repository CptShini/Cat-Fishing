using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance { get; private set; }
    [SerializeField] private Animator animator;

    [SerializeField] private Image notificationImage;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float speed = 1;
    [SerializeField] private float duration = 3;
    private float curDuration = 0;
    private bool started = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DiscoveryManager.Instance.OnDiscovered.AddListener(Discovery);
        DiscoveryManager.Instance.OnNewRecord.AddListener(Record);
    }

    private void Discovery(Fish fish)
    {
        text.text = "New Fish Discovered!";
        notificationImage.sprite = fish.Species.Icon;
        Notify();
    }

    private void Record(Fish fish)
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
        animator.SetBool("finished", false);
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            return;
        }
        if(started && curDuration > duration)
        {
            StopNotify();
        }
        else
        {
            curDuration += Time.deltaTime;
        }
    }
}
