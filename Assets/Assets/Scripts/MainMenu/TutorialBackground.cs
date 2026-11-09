
using UnityEngine;
using UnityEngine.UI;

public class TutorialBackground : MonoBehaviour
{
    [SerializeField] private GameObject tutorial;
    [SerializeField] private Image backgroundImage;

    [SerializeField, Range(0f, 1f)]
    private float transparency = 0.1f;

    private Color originalColor;

    private void Awake()
    {
        originalColor = backgroundImage.color;
    }

    private void Update()
    {
        Color color = originalColor;
        color.a = tutorial.activeSelf ? transparency : originalColor.a;

        backgroundImage.color = color;
    }
}
