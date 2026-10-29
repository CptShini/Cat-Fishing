using UnityEngine;
using UnityEngine.UI;

public class FishDragPreview : MonoBehaviour
{
    public static FishDragPreview Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image previewImage;

    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        if (previewImage == null)
            previewImage = GetComponent<Image>();

        previewImage.raycastTarget = false;
        gameObject.SetActive(false);
    }

    public void Show(Sprite sprite)
    {
        previewImage.sprite = sprite;
        previewImage.enabled = sprite != null;
        gameObject.SetActive(true);
    }

    public void Move(Vector2 screenPosition)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, uiCamera, out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
