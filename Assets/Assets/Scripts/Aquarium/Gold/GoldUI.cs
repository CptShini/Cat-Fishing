using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public static GoldUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI[] goldTexts;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateGold(int amount)
    {
        foreach (TextMeshProUGUI goldText in goldTexts)
        {
            if (!goldText) continue;
            
            goldText.text = amount.ToString();
        }
    }
}