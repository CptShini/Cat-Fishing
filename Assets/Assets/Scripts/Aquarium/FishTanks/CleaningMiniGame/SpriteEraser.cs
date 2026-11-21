
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTanks.CleaningMiniGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteEraser : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float brushRadius = 0.25f;
        [SerializeField, Range(0f, 1f)] private float brushStrength = 1f;
        [SerializeField, Range(0.1f, 1f)] private float brushSoftness = 0.65f;

        private SpriteRenderer _renderer;
        private Sprite _originalSprite;
        private Sprite _runtimeSprite;
        private Texture2D _runtimeTexture;

        private Color32[] _pixels;
        private Color32[] _originalPixels;

        private Rect _spriteRect;
        private int _textureWidth;
        private int _textureHeight;
        private float _pixelsPerUnit;
        private Vector2 _pivot;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _originalSprite = _renderer.sprite;

            Texture2D source = _originalSprite.texture;
            if (!source.isReadable)
            {
                Debug.LogError("SpriteEraser: sprite texture needs Read/Write Enabled.", this);
                enabled = false;
                return;
            }

            _runtimeTexture = Instantiate(source);
            _runtimeTexture.name = $"{source.name}_RuntimeErasable";

            _pixels = _runtimeTexture.GetPixels32();
            _originalPixels = (Color32[])_pixels.Clone();

            _textureWidth = _runtimeTexture.width;
            _textureHeight = _runtimeTexture.height;

            _spriteRect = _originalSprite.rect;
            _pixelsPerUnit = _originalSprite.pixelsPerUnit;
            _pivot = _originalSprite.pivot;

            _runtimeSprite = Sprite.Create(
                _runtimeTexture,
                _spriteRect,
                new(
                    _pivot.x / _spriteRect.width,
                    _pivot.y / _spriteRect.height),
                _pixelsPerUnit);

            _renderer.sprite = _runtimeSprite;
        }

        public bool EraseAt(Vector2 worldPosition)
        {
            if (_runtimeSprite == null)
                return false;

            Vector3 local = transform.InverseTransformPoint(worldPosition);

            Vector2 spritePoint = new(
                local.x * _pixelsPerUnit + _pivot.x,
                local.y * _pixelsPerUnit + _pivot.y);

            if (!_spriteRect.Contains(spritePoint))
                return false;

            int centerX = Mathf.RoundToInt(_spriteRect.x + spritePoint.x);
            int centerY = Mathf.RoundToInt(_spriteRect.y + spritePoint.y);

            int radius = Mathf.CeilToInt(brushRadius * _pixelsPerUnit);
            bool changed = false;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);

                    if (distance > radius)
                        continue;

                    int px = centerX + x;
                    int py = centerY + y;

                    if (px < 0 || px >= _textureWidth || py < 0 || py >= _textureHeight)
                        continue;

                    int index = py * _textureWidth + px;

                    if (_pixels[index].a == 0)
                        continue;

                    float normalizedDistance = distance / radius;

                    float softStart = 1f - brushSoftness;

                    float falloff = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - normalizedDistance) / brushSoftness));

                    float eraseAmount = falloff * brushStrength;

                    byte newAlpha = (byte)Mathf.RoundToInt(_pixels[index].a * (1f - eraseAmount));

                    if (newAlpha == _pixels[index].a)
                        continue;

                    _pixels[index].a = newAlpha;
                    changed = true;
                }
            }

            if (changed)
                ApplyPixels();

            return changed;
        }

        public float GetErasedPercentage()
        {
            if (_originalPixels == null || _pixels == null)
                return 0f;

            int originalPixelCount = 0;
            int erasedPixelCount = 0;

            for (int i = 0; i < _originalPixels.Length; i++)
            {
                if (_originalPixels[i].a == 0)
                    continue;

                originalPixelCount++;

                if (_pixels[i].a == 0)
                    erasedPixelCount++;
            }

            if (originalPixelCount == 0)
                return 0f;

            return (float)erasedPixelCount / originalPixelCount * 100f;
        }

        public byte[] GetAlphaData()
        {
            byte[] alpha = new byte[_pixels.Length];

            for (int i = 0; i < _pixels.Length; i++)
                alpha[i] = _pixels[i].a;

            return alpha;
        }

        public void LoadAlphaData(byte[] alpha)
        {
            if (alpha == null || alpha.Length != _pixels.Length)
            {
                ResetTexture();
                return;
            }

            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i].a = alpha[i];

            ApplyPixels();
        }

        public void ResetTexture()
        {
            if (_originalPixels == null)
                return;

            _pixels = (Color32[])_originalPixels.Clone();
            ApplyPixels();
        }

        private void ApplyPixels()
        {
            _runtimeTexture.SetPixels32(_pixels);
            _runtimeTexture.Apply(false);
        }

        private void OnDestroy()
        {
            if (_runtimeSprite)
                Destroy(_runtimeSprite);

            if (_runtimeTexture)
                Destroy(_runtimeTexture);
        }
    }
}
