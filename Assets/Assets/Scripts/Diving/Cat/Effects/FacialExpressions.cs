using System.Collections.Generic;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;

public class FacialExpressions : MonoBehaviour
{

    [SerializeField] private List<FacialExpression> _expressionsInitializer;

    private Dictionary<Emotion, Sprite> _expressions = new();

    private float duration;

    private float curDuration;
    private int curPriority;
    private bool finished = true;

    private Emotion curEmotion;

    private Emotion queuedEmotion;

    [SerializeField] private SpriteRenderer _face;
    [SerializeField] private InteractionDetector _interactionDetector;

    public readonly UnityEvent OnExpressible = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        foreach (var facialExpression in _expressionsInitializer)
        {
            _expressions.Add(facialExpression.emotion, facialExpression.sprite);
        }
    }

    void Start()
    {
        DivingController.Instance.OnStateChanged.AddListener(SetExpression);
        GetComponent<DamagedScript>().OnDamaged.AddListener(() => SetExpressionWithDuration(Emotion.HURT, 1f, 10));
        
        _face.sprite = _expressions[Emotion.DEFAULT];
        queuedEmotion = Emotion.DEFAULT;
        curEmotion = Emotion.DEFAULT;
       
        OnExpressible.AddListener(MakeExpressible);
        _interactionDetector.OnInteractibleEntered.AddListener(() => SetExpression(Emotion.CURIOUS, 5));
        _interactionDetector.OnInteractibleExited.AddListener(() => MakeExpressible());
    }

    public void SetExpression(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.STEALTH:
                SetExpression(Emotion.CURIOUS);
                break;
            case PlayerState.POUNCE:
                SetExpression(Emotion.CURIOUS);
                break;
            default:
                SetExpression(Emotion.DEFAULT);
                break;
        }
    }

    public void MakeExpressible()
    {
        curPriority = -10;
        SetExpression(queuedEmotion);
    }

    public bool SetExpression(Emotion emotion, int priority = 0) 
    {
        Debug.Log(emotion);
        if (priority < curPriority)
        {
            queuedEmotion = emotion;
            return false;
        }
        queuedEmotion = curEmotion;
        curPriority = priority;
        curEmotion = emotion;
        _face.sprite = _expressions[emotion];
        return true;
    }

    public void SetExpressionWithDuration(Emotion emotion, float duration, int priority)
    {
        if (!SetExpression(emotion, priority)) return;
        
        curDuration = 0;
        this.duration = duration;
        finished = false;
    }

    void Update()
    {
        if(curDuration > duration && !finished)
        {
            MakeExpressible();
            finished = true;
        }
        if(curDuration > duration)
        {
            return;
        }
        curDuration += Time.deltaTime;
    }
}
