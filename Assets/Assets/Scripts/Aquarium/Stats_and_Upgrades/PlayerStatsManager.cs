using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    // it is holding the player stats from the aquarium scene upgrades
    public static PlayerStatsManager Instance { get; private set; }

    [Header("Speed")]
    public float speed = 10;
    public float stealthSpeed = 3;
    public float pounceSpeed = 20;

    [Header("Acceleration")]
    public float acceleration = 5;
    public float friction = 5;

    [Header("Stamina")]
    public float maxStamina = 60;
    public float maxPounceStamina = 20;
    public float staminaRegen = 5;

    [Header("Strength")]
    public float strength = 10;

    [Header("Pouncing stats")]
    public float chargeUpSpeed = 10;
    public float pounceCooldown = 1;

    [Header("Oxygen")]
    public float oxygenCapacity = 100;

    [Header("Presence in Radius")]
    public float defaultPresence = 2;
    public float stealthPresence = 1;
    public float pouncePresence = 4;

    [Header("Other")]
    public float stunDuration = 2;
    public float carryingCapacity = 25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}