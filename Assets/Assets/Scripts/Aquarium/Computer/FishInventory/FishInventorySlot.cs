using Assets.Scripts.Data.Domain.Aquarium;
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

    public TankFish Fish { get; private set; }
    public static TankFish DraggedFish { get; private set; }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetFish(TankFish fish)
    {
        if (fish == null) return;

        Fish = fish;
        fishIcon.sprite = fish.Fish.Species.Icon;
        fishName.text = fish.Fish.Species.Name;
        fishIcon.enabled = fish.Fish.Species.Icon != null;
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Fish == null || fishIcon == null || fishIcon.sprite == null)
            return;

        if (canvas == null)
            return;

        DraggedFish = Fish;

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

        if (DraggedFish == Fish)
            DraggedFish = null;
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

        DraggedFish = null;
    }
}