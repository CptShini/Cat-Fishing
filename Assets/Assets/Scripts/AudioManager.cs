
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Aquarium Cat")]
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

    [Header("Diving Cat")]
    [SerializeField] private AudioClip catSwimSFX;
    [SerializeField] private AudioClip stealthSFX;
    [SerializeField] private AudioClip pounceSFX;

    [Header("Fish")]
    [SerializeField] private AudioClip fishSwimSFX;

    [Header("Battle")]
    [SerializeField] private AudioClip catchFishSuccessSFX;

    [Header("Hazard")]
    [SerializeField] private AudioClip hurtSFX;

    [Header("Music")]
    [SerializeField] private AudioClip battleBGM;




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

    // Play loop audio
    public void PlayLoop(AudioClip clip)
    {
        if (clip == null || loopSource == null)
            return;

        if (loopSource.clip == clip && loopSource.isPlaying)
            return;

        loopSource.clip = clip;
        loopSource.loop = true;
        loopSource.volume = 1f;
        loopSource.Play();
    }

    public void FadeOutLoop(float fadeDuration = 0.4f)
    {
        if (loopSource == null || !loopSource.isPlaying)
            return;

        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = loopSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loopSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        loopSource.Stop();
        loopSource.clip = null;
        loopSource.volume = startVolume;
    }

    // Audios
    public void PlayWalking()          { PlayLoop(walkSFX); }
    public void PlayCatSwimming()      { PlayLoop(catSwimSFX); }
    public void PlayFishSwimming()     { PlayLoop(fishSwimSFX); }
    public void PlayPurchaseSuccess()  { PlaySFX(purchaseSuccessSFX); }
    public void PlayNotEnoughGold()    { PlaySFX(notEnoughGoldSFX); }
    public void PlayInventoryFull()    { PlaySFX(inventoryFullSFX); }
    public void PlayAlreadyPurchased() { PlaySFX(alreadyPurchasedSFX); }
    public void PlayDoorOpen()         { PlaySFX(openingDoorSFX); }
    public void PlayButtonClick()      { PlaySFX(buttonClickSFX); }
    public void PlayPounce()           { PlaySFX(pounceSFX); }
    public void PlayStealth()          { PlaySFX(stealthSFX); }
    public void PlayCatchFishSuccess() { PlaySFX(catchFishSuccessSFX); }
    public void PlayHurt()             { PlaySFX(hurtSFX); }
    public void PlayBattleBGM()        { PlayLoop(battleBGM); }
}
