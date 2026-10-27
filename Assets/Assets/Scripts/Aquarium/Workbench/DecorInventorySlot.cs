using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorInventorySlot : MonoBehaviour
{
    public DecorItemSO decorItemSO;
    [SerializeField] private Image decorIcon;
    [SerializeField] private TMP_Text decorName;

    public void UpdateUI()
    {
        if (decorItemSO != null)
        {
            decorIcon.sprite = decorItemSO.decorIcon;
            decorIcon.gameObject.SetActive(true);
            decorName.text = decorItemSO.decorName;
            decorName.gameObject.SetActive(true);
        }
        else
        {
            decorIcon.gameObject.SetActive(false);
            decorName.gameObject.SetActive(false);
        }
    }
}
