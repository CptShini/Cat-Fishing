using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public static GoldUI Instance { get; private set; }

    [SerializeField] private TMP_Text[] goldTexts;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateGold(int amount)
    {
        foreach (TMP_Text goldText in goldTexts)
        {
            if (goldText != null)
                goldText.text = amount.ToString();
        }
    }
}