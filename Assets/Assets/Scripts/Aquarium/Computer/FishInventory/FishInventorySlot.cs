using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FishInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image fishIcon;
    [SerializeField] private TMP_Text fishName;

    private CanvasGroup canvasGroup;

    public FishSO Fish { get; private set; }

    public static FishSO DraggedFish { get; private set; }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetFish(FishSO fish)
    {
        if (fish == null) return;

        Fish = fish;
        fishIcon.sprite = fish.fishIcon;
        fishName.text = fish.fishName;
        fishIcon.enabled = fish.fishIcon != null;
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Fish == null || fishIcon.sprite == null)
            return;

        DraggedFish = Fish;

        if (FishDragPreview.Instance != null)
        {
            FishDragPreview.Instance.Show(fishIcon.sprite);
            FishDragPreview.Instance.Move(eventData.position);
        }

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (FishDragPreview.Instance != null)
            FishDragPreview.Instance.Move(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (FishDragPreview.Instance != null)
            FishDragPreview.Instance.Hide();

        DraggedFish = null;
    }
}
