using System.Collections.Generic;
using Assets.Scripts.PlayerState;
using UnityEngine;

public class FacialExpressions : MonoBehaviour
{

    [SerializeField] private List<FacialExpression> _expressionsInitializer;

    private Dictionary<Emotion, Sprite> _expressions = new();

    private float duration;

    private float curDuration;
    private bool finished = true;

    [SerializeField] private SpriteRenderer _face;

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
        GetComponent<DamagedScript>().OnDamaged.AddListener(() => SetExpressionWithDuration(Emotion.HURT, 1f));
        _face.sprite = _expressions[Emotion.DEFAULT];
    }

    public void SetExpression(PlayerState state)
    {
        if(curDuration < duration)
        {
            return;
        }
        switch (state)
        {
            case PlayerState.STEALTH:
                _face.sprite = _expressions[Emotion.CURIOUS];
                break;
            case PlayerState.POUNCE:
                _face.sprite = _expressions[Emotion.CURIOUS];
                break;
            
            default:
                _face.sprite = _expressions[Emotion.DEFAULT];
                break;
        }
    }

    public void SetExpression(Emotion emotion) 
    {
        if(curDuration < duration)
        {
            return;
        }
        _face.sprite = _expressions[emotion];
    }

    public void SetExpressionWithDuration(Emotion emotion, float duration)
    {
        _face.sprite = _expressions[emotion];
        curDuration = 0;
        this.duration = duration;
        finished = false;
    }

    void Update()
    {
        if(curDuration > duration && !finished)
        {
            finished = true;
            SetExpression(DivingController.Instance.state);
        }
        if(curDuration > duration)
        {
            return;
        }
        curDuration += Time.deltaTime;
    }
}
