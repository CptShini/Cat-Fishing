using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Aquarium.FishTanks.Interaction;
using UnityEngine.InputSystem;

public class SpongeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private FishInteractionController interactionController;
    [SerializeField] private RectTransform sponge;
    [SerializeField] private Canvas canvas;

    private Vector2 _startPosition;
    private RectTransform _parent;
    private bool _cleaning;

    private void Awake()
    {
        _parent = sponge.parent as RectTransform;
        _startPosition = sponge.anchoredPosition;
    }

     private void Update()
    {
        if (!_cleaning)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.isPressed)
        {
            StopCleaning();
            return;
        }

        Vector2 mousePosition = mouse.position.ReadValue();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parent,
                mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : canvas.worldCamera,
                out Vector2 localPoint))
        {
            sponge.anchoredPosition = localPoint;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (_cleaning)
            return;

        if (!RectTransformUtility.RectangleContainsScreenPoint(
                sponge,
                eventData.position,
                eventData.pressEventCamera))
            return;

        _cleaning = true;
        interactionController.SetMode(FishInteractionMode.CleanTank);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            StopCleaning();
    }

    private void StopCleaning()
    {
        if (!_cleaning)
            return;

        _cleaning = false;
        sponge.anchoredPosition = _startPosition;

        interactionController.SetMode(FishInteractionMode.MoveFish);
    }

    private void OnDisable()
    {
        StopCleaning();
    }
}
