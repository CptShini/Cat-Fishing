using Assets.Scripts.Data.Domain.Aquarium;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTank.FishInventory
{
    public class FishInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image fishIcon;
        [SerializeField] private TMP_Text fishName;

        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private GameObject dragVisual;

        private TankFish _fish;
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

            _fish = fish;
            fishIcon.sprite = fish.Fish.Species.Icon;
            fishName.text = $"{fish.Fish.Species.Name}\n{fish.Fish.Weight:F1} KG";
            fishIcon.enabled = fish.Fish.Species.Icon != null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_fish == null || fishIcon == null || fishIcon.sprite == null)
                return;

            if (canvas == null)
                return;

            DraggedFish = _fish;

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

            if (DraggedFish == _fish)
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
}