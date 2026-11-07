
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Player")]
    [SerializeField] private AudioClip walkSFX;

    [Header("Shop")]
    [SerializeField] private AudioClip purchaseSuccessSFX;
    [SerializeField] private AudioClip notEnoughGoldSFX;
    [SerializeField] private AudioClip inventoryFullSFX;
    [SerializeField] private AudioClip alreadyPurchasedSFX;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClickSFX;

    [Header("Doors")]
    [SerializeField] private AudioClip openingDoorSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Play a one-shot sound
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayWalking()
    {
        if (walkSFX == null || loopSource == null)
            return;

        if (!loopSource.isPlaying)
        {
            loopSource.clip = walkSFX;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    public void StopWalking()
    {
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.Stop();
        }                                        
    }

    // Shop
    public void PlayPurchaseSuccess()
    {
        PlaySFX(purchaseSuccessSFX);
    }

    public void PlayNotEnoughGold()
    {
        PlaySFX(notEnoughGoldSFX);
    }

    public void PlayInventoryFull()
    {
        PlaySFX(inventoryFullSFX);
    }

    public void PlayAlreadyPurchased()
    {
        PlaySFX(alreadyPurchasedSFX);
    }

    // Doors
    public void PlayDoorOpen()
    {
        PlaySFX(openingDoorSFX);
    }

    // UI
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }    
}
