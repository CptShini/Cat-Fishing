using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Aquarium.FishTanks.CleaningMiniGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteEraser : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float brushRadius = 0.25f;
        [SerializeField, Range(0f, 1f)] private float brushStrength = 1f;
        [SerializeField, Range(0.1f, 1f)] private float brushSoftness = 0.65f;

        [SerializeField, Min(0.01f)] private float applyInterval = 0.05f; //it will limit how often modified pixel data is uploaded to the GPU

        private SpriteRenderer _renderer;
        private Sprite _originalSprite;
        private Sprite _runtimeSprite;
        private Texture2D _runtimeTexture;

        private Color32[] _pixels;

        private Rect _spriteRect;
        private int _textureWidth;
        private int _textureHeight;
        private float _pixelsPerUnit;
        private Vector2 _pivot;

        private int _originalPixelCount;
        private int _erasedPixelCount;

        private float _lastApplyTime;
        private bool _pendingApply;

        // precalculate brush offsets and erase strengths to do it once and not in gameloop
        private struct BrushPixel
        {
            public int X;
            public int Y;
            public float EraseAmount;

            public BrushPixel(int x, int y, float eraseAmount)
            {
                X = x;
                Y = y;
                EraseAmount = eraseAmount;
            }
        }

        private BrushPixel[] _brushPixels;

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

            _textureWidth = _runtimeTexture.width;
            _textureHeight = _runtimeTexture.height;

            _spriteRect = _originalSprite.rect;
            _pixelsPerUnit = _originalSprite.pixelsPerUnit;
            _pivot = _originalSprite.pivot;

            _runtimeSprite = Sprite.Create(_runtimeTexture, _spriteRect, new Vector2(_pivot.x / _spriteRect.width, _pivot.y / _spriteRect.height), _pixelsPerUnit);

            _renderer.sprite = _runtimeSprite;

            CountOriginalPixels();
            BuildBrush();
        }

        // for counting original non-transparent pixels
        private void CountOriginalPixels()
        {
            _originalPixelCount = 0;

            int xMin = Mathf.RoundToInt(_spriteRect.x);
            int yMin = Mathf.RoundToInt(_spriteRect.y);
            int xMax = xMin + Mathf.RoundToInt(_spriteRect.width);
            int yMax = yMin + Mathf.RoundToInt(_spriteRect.height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (_pixels[y * _textureWidth + x].a > 0)
                        _originalPixelCount++;
                }
            }

            _erasedPixelCount = 0;
        }

        //calculates brush shape once instead of every time we erase
        private void BuildBrush()
        {
            int radius = Mathf.CeilToInt(brushRadius * _pixelsPerUnit);
            int radiusSquared = radius * radius;

            List<BrushPixel> brush = new();

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int distanceSquared = x * x + y * y;

                    if (distanceSquared > radiusSquared)
                        continue;

                    float distance = Mathf.Sqrt(distanceSquared);
                    float normalizedDistance = distance / radius;

                    float falloff = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - normalizedDistance) / brushSoftness));

                    float eraseAmount = falloff * brushStrength;

                    if (eraseAmount <= 0f)
                        continue;

                    brush.Add(new BrushPixel(x, y, eraseAmount));
                }
            }

            _brushPixels = brush.ToArray();
        }

        public bool EraseAt(Vector2 worldPosition)
        {
            if (_runtimeSprite == null)
                return false;

            Vector3 local = transform.InverseTransformPoint(worldPosition);

            Vector2 spritePoint = new(local.x * _pixelsPerUnit + _pivot.x, local.y * _pixelsPerUnit + _pivot.y);

            if (!_spriteRect.Contains(spritePoint))
                return false;

            int centerX = Mathf.RoundToInt(_spriteRect.x + spritePoint.x);
            int centerY = Mathf.RoundToInt(_spriteRect.y + spritePoint.y);

            bool changed = false;

            // reuse the precalculated brush pixels
            foreach (BrushPixel brushPixel in _brushPixels)
            {
                int px = centerX + brushPixel.X;
                int py = centerY + brushPixel.Y;

                if (px < 0 || px >= _textureWidth || py < 0 || py >= _textureHeight)
                    continue;

                int index = py * _textureWidth + px;

                byte oldAlpha = _pixels[index].a;

                if (oldAlpha == 0)
                    continue;

                byte newAlpha = (byte)Mathf.RoundToInt(oldAlpha * (1f - brushPixel.EraseAmount));

                if (newAlpha == oldAlpha)
                    continue;

                _pixels[index].a = newAlpha;

                if (newAlpha == 0)
                    _erasedPixelCount++;

                changed = true;
            }

            if (changed)
                RequestApply();

            return changed;
        }

        public float GetErasedPercentage()
        {
            if (_originalPixelCount == 0)
                return 0f;

            return (float)_erasedPixelCount / _originalPixelCount * 100f;
        }

        public void ResetTexture()
        {
            if (_originalSprite == null)
                return;

            _pixels = _originalSprite.texture.GetPixels32();

            CountOriginalPixels();

            ApplyPixels();
            _pendingApply = false;
        }

        private void RequestApply()
        {
            _pendingApply = true;

            if (Time.time - _lastApplyTime < applyInterval)
                return;

            ApplyPixels();
        }

        private void LateUpdate()
        {
            if (!_pendingApply)
                return;

            if (Time.time - _lastApplyTime >= applyInterval)
                ApplyPixels();
        }

        private void ApplyPixels()
        {
            _runtimeTexture.SetPixels32(_pixels);
            _runtimeTexture.Apply(false);

            _lastApplyTime = Time.time;
            _pendingApply = false;
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
