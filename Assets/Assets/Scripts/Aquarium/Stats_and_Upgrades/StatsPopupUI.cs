using UnityEngine;
using UnityEngine.InputSystem;

public class StatsPopupUI : MonoBehaviour
{
    public static StatsPopupUI Instance { get; private set; }

    private StatPopup currentPopup;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentPopup == null)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RectTransform popupRect = currentPopup.PopupRect;

            Canvas canvas = popupRect.GetComponentInParent<Canvas>();

            Camera uiCamera = null;

            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = canvas.worldCamera;
            }

            bool clickedInside = RectTransformUtility.RectangleContainsScreenPoint(popupRect, mousePosition, uiCamera);

            if (!clickedInside)
            {
                HidePopup();
            }
        }
    }

    public void ShowPopup(StatPopup popup)
    {
        if (currentPopup != null)
        {
            currentPopup.Hide();
            currentPopup.SetSelected(false);
        }

        currentPopup = popup;
        currentPopup.Show();
        currentPopup.SetSelected(true);
    }

    public void HidePopup()
    {
        if (currentPopup == null)
            return;

        currentPopup.Hide();
        currentPopup.SetSelected(false);
        currentPopup = null;
    }
}