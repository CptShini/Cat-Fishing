using UnityEngine;

namespace Assets.Scripts.Fishies.Perception
{
    [RequireComponent(typeof(FishPerception))]
    public class FishPerceptionVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer perceptionImage;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Color 
            detectedColor,
            detectingColor,
            notDetectedColor;

        [SerializeField] private float cooldownTime = 1f;
        
        private Vector2 _initialSize;
        private Transform _transform, _imageTransform;
        private FishPerception _perception;

        private bool _cooldownActive;
        private float _cooldownTimer;

        private float _fillAmount;
        private Color _color;
        
        private float _lastFillAmount = float.NaN;
        private Color _lastColor;
        private bool _hasAppliedColor;

        private void Awake()
        {
            _perception = GetComponent<FishPerception>();
            _initialSize = perceptionImage.size;
            _transform = transform;
            _imageTransform = perceptionImage.transform;
        }

        private void Update()
        {
            UpdateVisualState();
            ApplyVisualState();
        }

        private void UpdateVisualState()
        {
            if (_cooldownActive)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_perception.ReactionPercent > 0f)
                    _cooldownTimer = cooldownTime;
                else if (_cooldownTimer <= 0f)
                    _cooldownActive = false;

                return;
            }

            float reaction = _perception.ReactionPercent;
            if (reaction <= 0f)
            {
                _fillAmount = 0f;
                _color = notDetectedColor;
                return;
            }

            _fillAmount = Mathf.Clamp01(reaction);
            _color = _perception.Detecting ? detectingColor : notDetectedColor;

            if (!_perception.Cooldown)
                return;
            
            _fillAmount = 1f;
            _color = detectedColor;
            
            _cooldownActive = true;
            _cooldownTimer = cooldownTime;
        }

        private void ApplyVisualState()
        {
            if (!Mathf.Approximately(_lastFillAmount, _fillAmount))
            {
                perceptionImage.size = new(_initialSize.x, _initialSize.y * _fillAmount);
                _lastFillAmount = _fillAmount;
            }
            
            if (!_hasAppliedColor || _lastColor != _color)
            {
                perceptionImage.color = _color;

                _lastColor = _color;
                _hasAppliedColor = true;
            }
            
            _imageTransform.position =
                _transform.position +
                _transform.right * offset.x +
                Vector3.up * offset.y;
            _imageTransform.rotation = Quaternion.identity;
        }
    }
}