using Assets.Scripts.Audio.SFX;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    public Sprite activeTab;
    public Sprite inactiveTab;

    [Header("Tutorials")]
    [SerializeField] private string[] tutorialIds;

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].sprite = inactiveTab;
        }
        pages[tabIndex].SetActive(true);
        tabImages[tabIndex].sprite = activeTab;
        SFXManager.Instance.Play(SFXId.ButtonClick);

        // show tutorial for selected tab
        if (tutorialIds != null && tabIndex < tutorialIds.Length && !string.IsNullOrEmpty(tutorialIds[tabIndex]))
        {
            TutorialUIController.Instance.ShowTutorial(tutorialIds[tabIndex]);
        }
    }
}