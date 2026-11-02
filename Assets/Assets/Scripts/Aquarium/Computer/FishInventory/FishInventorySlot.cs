using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FishInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image fishIcon;
    [SerializeField] private TMP_Text fishName;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private GameObject dragVisual;

    public FishDTO FishDto { get; private set; }
    public static FishDTO DraggedFishDto { get; private set; }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetFish(FishDTO fishDto)
    {
        if (fishDto == null) return;

        FishDto = fishDto;
        fishIcon.sprite = fishDto.sprite;
        fishName.text = fishDto.Name;
        fishIcon.enabled = fishDto.sprite != null;
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        if (FishDto == null || fishIcon == null || fishIcon.sprite == null)
            return;

        if (canvas == null)
            return;

        DraggedFishDto = FishDto;

        dragVisual = new GameObject("FishDragVisual");
        dragVisual.transform.SetParent(canvas.transform, false);
        dragVisual.transform.SetAsLastSibling();

        Image image = dragVisual.AddComponent<Image>();
        image.sprite = fishIcon.sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        RectTransform dragRect = dragVisual.GetComponent<RectTransform>();
        dragRect.sizeDelta = fishIcon.rectTransform.rect.size;
        dragRect.position = eventData.position;

        CanvasGroup dragCanvasGroup = dragVisual.AddComponent<CanvasGroup>();
        dragCanvasGroup.alpha = 0.7f;
        dragCanvasGroup.blocksRaycasts = false;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragVisual != null)
            dragVisual.transform.position = eventData.position;
    }

    
    private void OnDisable()
    {
        if (dragVisual != null)
        {
            Destroy(dragVisual);
            dragVisual = null;
        }

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (DraggedFishDto == FishDto)
            DraggedFishDto = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragVisual != null)
        {
            Destroy(dragVisual);
            dragVisual = null;
        }

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        DraggedFishDto = null;
    }
}