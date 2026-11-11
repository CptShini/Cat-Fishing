
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject[] tutorialImages;
    [SerializeField] private GameObject[] pagesImages;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float selectedPageScale = 1.2f;
    [SerializeField] private Color selectedPageColor = new Color32(255, 255, 255, 200);
    // [SerializeField] private Color unselectedPageColor = new Color32(150, 150, 150, 200);
    [SerializeField] private Color unselectedPageColor = new Color32(120, 120, 120, 200);


    [SerializeField, Range(0f, 1f)]
    private float transparency = 0.1f;

    private int currentImageIndex = 0;
    private Color originalColor;

    private void Awake()
    {
        originalColor = backgroundImage.color;
    }

    private void Start()
    {
        ShowImage(0);
    }

    private void Update()
    {
        UpdateBackground();

        if (!tutorial.activeSelf || Keyboard.current == null)
            return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            PreviousImage();

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            NextImage();
    }

    private void UpdateBackground()
    {
        Color color = originalColor;
        color.a = tutorial.activeSelf ? transparency : originalColor.a;
        backgroundImage.color = color;
    }

    public void SelectPage(int index)
    {
        if (index < 0 || index >= tutorialImages.Length)
            return;

        ShowImage(index);
    }

    public void NextImage()
    {
        if (tutorialImages.Length == 0)
            return;

        int nextIndex = (currentImageIndex + 1) % tutorialImages.Length;
        ShowImage(nextIndex);
    }

    public void PreviousImage()
    {
        if (tutorialImages.Length == 0)
            return;

        int previousIndex = (currentImageIndex - 1 + tutorialImages.Length) % tutorialImages.Length;
        ShowImage(previousIndex);
    }

    private void UpdatePagination()
    {
        for (int i = 0; i < pagesImages.Length; i++)
        {
            bool isSelected = i == currentImageIndex;

            Image pageImage = pagesImages[i].GetComponent<Image>();

            if (pageImage != null)
                pageImage.color = isSelected ? selectedPageColor : unselectedPageColor;

            pagesImages[i].transform.localScale = isSelected
                ? Vector3.one * selectedPageScale
                : Vector3.one;

            Transform glow = pagesImages[i].transform.Find("Glow");

            if (glow != null)
                glow.gameObject.SetActive(isSelected);
        }
    }
    
    private void ShowImage(int index)
    {
        currentImageIndex = index;

        for (int i = 0; i < tutorialImages.Length; i++) {
            tutorialImages[i].SetActive(i == currentImageIndex);
        }

        UpdatePagination();
    }

}
