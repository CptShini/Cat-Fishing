using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.gray; //inactive tab color
        }
        pages[tabIndex].SetActive(true);
        tabImages[tabIndex].color = Color.white;
        AudioManager.Instance.Play(AudioId.ButtonClick);
    }
}
