using UnityEngine;

public class FurnitureSplitRenderer : MonoBehaviour
{
    private SpriteRenderer originalRenderer;
    private SpriteRenderer bottomRenderer;
    private SpriteRenderer topRenderer;

    private Sprite bottomSprite;
    private Sprite topSprite;

    private bool initialized;

    public void Configure(ItemSO item)
    {
        if (item == null || !item.splitRendering)
            return;

        if (!initialized)
        {
            originalRenderer = GetComponentInChildren<SpriteRenderer>();

            CreateSplitSprites(originalRenderer.sprite, item.splitHeight);

            if (bottomSprite == null || topSprite == null)
                return;

            CreateRenderers();
            initialized = true;
        }

        string baseLayer = FurnitureSortingUtility.GetSortingLayer(item.sortingLayer);

        bottomRenderer.sortingLayerName = baseLayer;
        bottomRenderer.sortingOrder = item.sortingOrder;

        topRenderer.sortingLayerName = "AboveWorld";
        topRenderer.sortingOrder = item.sortingOrder;
    }

    private void CreateSplitSprites(Sprite source, float splitHeight)
    {
        Texture2D texture = source.texture;

        Rect sourceRect = source.rect;

        int width = Mathf.RoundToInt(sourceRect.width);
        int height = Mathf.RoundToInt(sourceRect.height);

        int splitPixel = Mathf.Clamp(Mathf.RoundToInt(height * splitHeight), 1, height - 1);

        Rect bottomRect = new Rect(sourceRect.x, sourceRect.y, width, splitPixel);
        Rect topRect = new Rect(sourceRect.x, sourceRect.y + splitPixel, width, height - splitPixel);
        Vector2 originalPivot = source.pivot;
        Vector2 bottomPivot = new Vector2(originalPivot.x / width, originalPivot.y / splitPixel);
        Vector2 topPivot = new Vector2(originalPivot.x / width, (originalPivot.y - splitPixel) / (height - splitPixel));

        bottomSprite = Sprite.Create(texture, bottomRect, bottomPivot, source.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        topSprite = Sprite.Create(texture, topRect, topPivot, source.pixelsPerUnit, 0, SpriteMeshType.FullRect);

        bottomSprite.name = source.name + "_Bottom";
        topSprite.name = source.name + "_Top";
    }

    private void CreateRenderers()
    {
        GameObject bottomObject = new GameObject("BottomVisual");
        bottomObject.transform.SetParent(originalRenderer.transform, false);

        GameObject topObject = new GameObject("TopVisual");
        topObject.transform.SetParent(originalRenderer.transform, false);

        bottomRenderer = bottomObject.AddComponent<SpriteRenderer>();
        topRenderer = topObject.AddComponent<SpriteRenderer>();

        CopyRendererSettings(originalRenderer, bottomRenderer);
        CopyRendererSettings(originalRenderer, topRenderer);

        bottomRenderer.sprite = bottomSprite;
        topRenderer.sprite = topSprite;

        originalRenderer.enabled = false;
    }

    private void CopyRendererSettings(SpriteRenderer source, SpriteRenderer target)
    {
        target.color = source.color;
        target.sharedMaterial = source.sharedMaterial;
        target.flipX = source.flipX;
        target.flipY = source.flipY;
        target.maskInteraction = source.maskInteraction;
        target.sortingOrder = source.sortingOrder;
    }

    private void OnDestroy()
    {
        if (bottomSprite != null)
            Destroy(bottomSprite);

        if (topSprite != null)
            Destroy(topSprite);
    }
}