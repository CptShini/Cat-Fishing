using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    public Sprite activeTab;
    public Sprite inactiveTab;

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
    }
}