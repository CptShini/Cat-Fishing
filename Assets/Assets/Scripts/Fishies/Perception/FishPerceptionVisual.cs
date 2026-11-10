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
        private FishPerception _perception;

        private bool _cooldownActive;
        private float _cooldownTimer;

        private float _fillAmount;
        private Color _color;
        
        private void Awake()
        {
            _perception = GetComponent<FishPerception>();
            _initialSize = perceptionImage.size;
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
                if (_perception.ReactionPercent > 0f) _cooldownTimer = cooldownTime;
                else if (_cooldownTimer <= 0f) _cooldownActive = false;
                return;
            }

            if (_perception.ReactionPercent <= 0f)
            {
                _fillAmount = 0f;
                return;
            }

            _fillAmount = _perception.ReactionPercent;
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
            perceptionImage.size = new(_initialSize.x, _initialSize.y * _fillAmount);
            perceptionImage.color = _color;

            perceptionImage.transform.position =
                transform.position +
                transform.right * offset.x +
                Vector3.up * offset.y;
            perceptionImage.transform.rotation = Quaternion.identity;
        }
    }
}