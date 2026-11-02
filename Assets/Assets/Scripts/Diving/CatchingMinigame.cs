using System;
using Assets.Scripts.Diving;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.CatchMinigame;
using Assets.Scripts.Fish.Domain;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CatchingMinigame : MonoBehaviour
{
    public static CatchingMinigame Instance { get; private set; }

    private FishDTO _fishDto;
    private FishCaughtMovement movement;

    [SerializeField] private StatBar catchBar;
    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private GameObject catAndFish;
    [SerializeField] private Image fishImage;
    private float contestThreshold = 5; // The value that contest must be to catch a fish succesfully
    private float fishAngle;
    
    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Init(FishDTO fishDto, FishCaughtMovement movement)
    {
        this._fishDto = fishDto;
        this.movement = movement;
        
        fishImage.sprite = fishDto.sprite;
        minigameCanvas.SetActive(true);

        catchBar.SetMaxValue(contestThreshold, contestThreshold / 2);
    }

    public void Grab(Vector2 velocityVector)
    {
        float angle = Vector2.Angle(velocityVector, movement.Direction);
        float power = DivingPlayerStats.Instance.strength * (angle / 180);
        float contest = Math.Clamp((power - _fishDto.Strength) / _fishDto.Strength, -10, 5);
        catchBar.CurrentValue += contest * Time.deltaTime;

        if (catchBar.CurrentValue >= contestThreshold)
        {
            //CATCH FISH
            FishCollectible fishCol = new(_fishDto);
            if (DivingInventory.Instance.Add(fishCol)) // Checks if putting in inventory is successful
            {
                Destroy(movement.gameObject);
                DivingController.Instance.SetPlayerState(PlayerState.SWIM);
            }
            else // Cat cannot win, if their inventory is full
            {
                DivingController.Instance.SetPlayerState(PlayerState.STUN);
                movement.Lose(DivingController.Instance.gameObject.transform.position);
            }
            minigameCanvas.SetActive(false);

        } else if (catchBar.CurrentValue <= 0)
        {
            //LOSE FISH
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            movement.Lose(DivingController.Instance.gameObject.transform.position);
            minigameCanvas.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (DivingController.Instance.state != PlayerState.GRAB)
            return;
        
        float angleRad = Mathf.Atan2(movement.Direction.y, movement.Direction.x);
        fishAngle = angleRad * (180 / Mathf.PI);
        catAndFish.transform.eulerAngles = new Vector3(0,0, fishAngle);
    }
}
