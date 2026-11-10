using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatPopup : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject popUp;
    [SerializeField] private Image[] slotImages;

    [SerializeField] private float selectedScale = 1.08f;
    private Vector3 originalScale;
    [SerializeField] private Transform slotVisual;

    public RectTransform PopupRect { get; private set; }

    private void Awake()
    {
        originalScale = transform.localScale;

        PopupRect = popUp.GetComponent<RectTransform>();

        popUp.SetActive(false);
        SetSelected(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StatsPopupUI.Instance.ShowPopup(this);
    }

    public void Show()
    {
        popUp.SetActive(true);
    }

    public void Hide()
    {
        popUp.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        Color color = selected ? Color.white : new Color32(233, 233, 233, 255);

        foreach (Image img in slotImages)
        {
            img.color = color;
        }

        slotVisual.transform.localScale = selected ? originalScale * selectedScale : originalScale;
    }
}