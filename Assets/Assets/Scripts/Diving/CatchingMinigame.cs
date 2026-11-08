using System;
using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CatchingMinigame : MonoBehaviour
{
    public static CatchingMinigame Instance { get; private set; }

    private float angleRad = 0;
    private float wiggleRad = 0;

    [SerializeField] private float inaccuracyPenalty = 3;

    [SerializeField] private float wiggleRange = 25;
    [SerializeField] private float wiggleSpeed = 250;
    float wiggleTime = 0;
    private bool flipped = false;
    private Fish _fish;
    private FishCaughtMovement movement;

    [SerializeField] private StatBar catchBar;
    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private GameObject catAndFish;
    [SerializeField] private Image fishImage;
    private float contestThreshold = 5; // The value that contest must be to catch a fish succesfully
    private float fishAngle;

    public readonly UnityEvent<bool> OnEnded = new();
    
    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Init(Fish fish, FishCaughtMovement movement)
    {
        _fish = fish;
        this.movement = movement;
        
        fishImage.sprite = fish.Species.Icon;
        minigameCanvas.SetActive(true);

        catchBar.SetMaxValue(contestThreshold, contestThreshold / 2);
        angleRad = ((Mathf.Atan2(movement.Direction.y, movement.Direction.x) * Mathf.Rad2Deg) + 360f) % 360f;
        wiggleTime = 0;
        wiggleRad = 0;
    }

    public void Grab(Vector2 velocityVector)
    {
        float angle = Vector2.Angle(velocityVector, movement.Direction);
        float power = DivingPlayerStats.Instance.strength * Mathf.Pow(angle / 180, inaccuracyPenalty);
        Debug.Log($"{angle / 180}, {Mathf.Pow(angle / 180, inaccuracyPenalty)}");
        float contest = Math.Clamp((power - _fish.Strength) / _fish.Strength, -10, 5);
        catchBar.CurrentValue += contest * Time.deltaTime;

        if (catchBar.CurrentValue >= contestThreshold)
        {
            //CATCH FISH
            FishCollectible fishCol = new(_fish);
            if (DivingInventory.Instance.Add(fishCol)) // Checks if putting in inventory is successful
            {
                Destroy(movement.gameObject);
                DivingController.Instance.SetPlayerState(PlayerState.SWIM);
                OnEnded.Invoke(true);
            }
            else // Cat cannot win, if their inventory is full
            {
                DivingController.Instance.SetPlayerState(PlayerState.STUN);
                movement.Lose(DivingController.Instance.gameObject.transform.position);
                OnEnded.Invoke(false);
            }
            minigameCanvas.SetActive(false);

        } else if (catchBar.CurrentValue <= 0)
        {
            //LOSE FISH
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            movement.Lose(DivingController.Instance.gameObject.transform.position);
            minigameCanvas.SetActive(false);
            OnEnded.Invoke(false);
        }
    }
    
    private void Rotate()
    {
        float targetRotation = ((Mathf.Atan2(movement.Direction.y, movement.Direction.x) * Mathf.Rad2Deg) + 360f) % 360f;

        float newRotation = Mathf.MoveTowardsAngle(
            angleRad,
            targetRotation,
            Time.deltaTime * 500
        );

        angleRad += Mathf.DeltaAngle(angleRad, newRotation);
        
        if(!flipped && 90f < (angleRad + 360) % 360 && (angleRad + 360) % 360 <= 268f)
        {
            flipped = true;
            catAndFish.transform.localScale = new Vector3 (1f, -1f, 1);
            return;
        }
        else if(flipped && (!(90f < (angleRad + 360) % 360) || !((angleRad + 360) % 360 <= 272f)))
        {
            flipped = false;
            catAndFish.transform.localScale = new Vector3 (1f, 1f, 1);
        }
    }

    private void Wiggle()
    {
        wiggleTime += Time.deltaTime * wiggleSpeed;
        wiggleRad = Mathf.Cos(wiggleTime) * wiggleRange;
    }

    private void SetAngle(float angle)
    {
        catAndFish.transform.eulerAngles = new Vector3(0,0, angle);
    }

    private void Update()
    {
        if (DivingController.Instance.state != PlayerState.GRAB)
            return;
        Rotate();
        Wiggle();
        SetAngle(wiggleRad + angleRad);
    }
}
