
using System;
using System.Runtime.CompilerServices;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.UI;

public class CatchingMinigame : MonoBehaviour
{

    public static CatchingMinigame Instance { get; private set; }

    private Fish fish;
    private FishCaughtMovement movement;

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

        gameObject.SetActive(false);
    }

    public void Init(Fish fish, FishCaughtMovement movement)
    {
        this.fish = fish;
        this.movement = movement;
        //fishImage.sprite = fish.sprite;
        gameObject.SetActive(true);

        CatchBar.Instance.SetMaxValue(contestThreshold);
        CatchBar.Instance.SetCurrentValue(contestThreshold / 2);
    }

    public void Grab(Vector2 velocityVector)
    {
        float angle = Vector2.Angle(velocityVector, movement.direction);
        float power = DivingPlayerStats.Instance.strength * (angle / 180);
        float contest = Math.Clamp((power - fish.strength) / fish.strength, -10, 5);
        CatchBar.Instance.AddToCurrentValue(contest * Time.deltaTime);

        if( CatchBar.Instance.currentValue >= contestThreshold)
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
                movement.Loose(DivingController.Instance.gameObject.transform.position);
            }
            gameObject.SetActive(false);

        } else if(CatchBar.Instance.currentValue <= 0)
        {
            //LOSE FISH
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            movement.Loose(DivingController.Instance.gameObject.transform.position);
            gameObject.SetActive(false);
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
        float angleRad = Mathf.Atan2(movement.direction.y, movement.direction.x);
        fishAngle = angleRad * (180 / Mathf.PI);
        catAndFish.transform.eulerAngles = new Vector3(0,0, fishAngle);
    }
}
