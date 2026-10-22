
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image staminaForeground;
    [SerializeField, Range(0f, 100f)] private float currentStamina = 100f;

    void Update()
    {
        // placeholder logic
        staminaForeground.fillAmount = currentStamina / 100f;
    }
}
