
using UnityEngine;
using UnityEngine.UI;

public class CatchingMinigame : MonoBehaviour
{

    public static CatchingMinigame Instance { get; private set; }

    [SerializeField] private GameObject catAndFish;
    [SerializeField] private Image fishImage;
    [SerializeField] private Image catImage;

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;

        gameObject.SetActive(false);
    }

    public void Init(Fish fish)
    {
        fishImage.sprite = fish.sprite;
        gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
