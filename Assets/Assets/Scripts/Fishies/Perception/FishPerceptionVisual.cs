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

        private Vector2 _initialSize;
        private Quaternion _initialRotation;
        private FishPerception _perception;
        
        private void Awake()
        {
            _perception = GetComponent<FishPerception>();
            _initialSize = perceptionImage.size;
            _initialRotation = perceptionImage.transform.rotation;
        }

        private void Update()
        {
            bool visualActive = _perception.ReactionPercent > 0f;
            if (!visualActive)
            {
                perceptionImage.size = Vector2.zero;
                return;
            }

            float fillAmount;
            Color color;

            if (_perception.Cooldown)
            {
                fillAmount = 1f;
                color = detectedColor;
            }
            else
            {
                fillAmount = _perception.ReactionPercent;
                color = _perception.Detecting ? detectingColor : notDetectedColor;
            }
            
            perceptionImage.size = new(_initialSize.x, _initialSize.y * fillAmount);
            perceptionImage.color = color;
            perceptionImage.transform.position = transform.position + transform.right + offset;
            perceptionImage.transform.rotation = _initialRotation;
        }
    }
}