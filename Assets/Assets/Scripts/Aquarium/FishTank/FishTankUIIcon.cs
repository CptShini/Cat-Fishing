using UnityEngine;
using UnityEngine.UI;

public class FishTankUIIcon : MonoBehaviour
{
    [SerializeField] private Image tankImage;

    [Header("Tank object on the map")]
    [SerializeField] private FishTank fishTank;

    public FishTank Tank => fishTank;

    private void Awake()
    {
        if (tankImage == null)
            tankImage = GetComponent<Image>();
    }

    public void SelectTank()
    {
        if (fishTank == null)
        {
            Debug.LogWarning("FishTank is not assigned to this icon!", this);
            return;
        }

        Debug.Log("Selected aquarium: " + fishTank.gameObject.name);
    }
}
