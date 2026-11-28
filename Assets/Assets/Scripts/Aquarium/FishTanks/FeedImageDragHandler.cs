using Assets.Scripts.Aquarium.FishTanks.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FeedImageDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private FishInteractionController interactionController;

        private RectTransform _rectTransform;
        private RectTransform _canvasRect;
        private Canvas _canvas;

        private Transform _originalParent;
        private Vector2 _originalPosition;
        private int _originalSiblingIndex;

        private Button _button;

        private bool _dragging;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rectTransform = GetComponent<RectTransform>();

            _canvas = GetComponentInParent<Canvas>();
            if (_canvas)
                _canvas = _canvas.rootCanvas;

            if (_canvas)
                _canvasRect = _canvas.transform as RectTransform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!interactionController ||
                !interactionController.isActiveAndEnabled ||
                !_canvasRect ||
                !_button.interactable)
                return;

            _originalParent = _rectTransform.parent;
            _originalPosition = _rectTransform.anchoredPosition;
            _originalSiblingIndex = _rectTransform.GetSiblingIndex();

            _rectTransform.SetParent(_canvas.transform, true);
            _rectTransform.SetAsLastSibling();

            _dragging = true;

            interactionController.BeginFeedDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging)
                return;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, eventData.position, eventData.pressEventCamera, out Vector3 worldPosition))
            {
                _rectTransform.position = worldPosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging)
                return;

            _dragging = false;

            interactionController.EndFeedDrag(eventData.position);

            _rectTransform.SetParent(_originalParent, false);
            _rectTransform.SetSiblingIndex(_originalSiblingIndex);
            _rectTransform.anchoredPosition = _originalPosition;
        }

        private void OnDisable()
        {
            if (!_dragging)
                return;

            _dragging = false;

            if (interactionController != null &&
                interactionController.isActiveAndEnabled)
            {
                interactionController.SetMode(FishInteractionMode.MoveFish);
            }

            if (!_originalParent) return;
            
            _rectTransform.SetParent(_originalParent, false);
            _rectTransform.SetSiblingIndex(_originalSiblingIndex);
            _rectTransform.anchoredPosition = _originalPosition;
        }
    }
}