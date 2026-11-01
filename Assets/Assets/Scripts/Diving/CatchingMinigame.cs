using System;
using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.CatchMinigame;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.UI;

public class CatchingMinigame : MonoBehaviour
{
    public static CatchingMinigame Instance { get; private set; }

    private Fish fish;
    private FishCaughtMovement movement;

    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private GameObject catAndFish;
    [SerializeField] private Image fishImage;
    [SerializeField] private Image catImage;
    private float contestThreshold = 5; // The value that contest must be to catch a fish succesfully
    private float fishAngle;

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Init(Fish fish, FishCaughtMovement movement)
    {
        this.fish = fish;
        this.movement = movement;
        //fishImage.sprite = fish.sprite;
        minigameCanvas.SetActive(true);

        CatchBar.Instance.SetMaxValue(contestThreshold, contestThreshold / 2);
    }

    public void Grab(Vector2 velocityVector)
    {
        float angle = Vector2.Angle(velocityVector, movement.Direction);
        float power = DivingPlayerStats.Instance.strength * (angle / 180);
        float contest = Math.Clamp((power - fish.strength) / fish.strength, -10, 5);
        CatchBar.Instance.CurrentValue += contest * Time.deltaTime;

        if( CatchBar.Instance.CurrentValue >= contestThreshold)
        {
            //CATCH FISH
            FishCollectible fishCol = new FishCollectible(fish);
            if(DivingInventory.Instance.Add(fishCol)) // Checks if putting in inventory is successful
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

        } else if(CatchBar.Instance.CurrentValue <= 0)
        {
            //LOSE FISH
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            movement.Lose(DivingController.Instance.gameObject.transform.position);
            minigameCanvas.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DivingController.Instance.state != PlayerState.GRAB)
        {
            return;
        }
        float angleRad = Mathf.Atan2(movement.Direction.y, movement.Direction.x);
        fishAngle = angleRad * (180 / Mathf.PI);
        catAndFish.transform.eulerAngles = new Vector3(0,0, fishAngle);
    }
}
