using System;
using Assets.Scripts.Fish.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishTankFishRow : MonoBehaviour
{
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private Button returnButton;

    public void Setup(Fish fish, Action onReturn)
    {
        fishNameText.text = fish.Species.Name;

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => onReturn?.Invoke());
    }
}
