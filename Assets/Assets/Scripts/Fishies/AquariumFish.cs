using System.Collections;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    public class AquariumFish : MonoBehaviour
    {
        public TankFish Fish { get; private set; }

        private bool _hovering;
        private bool _showingHappinessChange;
        private FishNormalMovement _movement;
        private Coroutine _happinessImageRoutine;

        [SerializeField] private FishVisuals visuals;
        [SerializeField] private Image happinessImage;
        [SerializeField] private Sprite
            loveSprite,
            happySprite,
            neutralSprite,
            sadSprite,
            cryingSprite;

        [SerializeField] private float happinessDisplayTime = 1f;
        [SerializeField] private float happinessFadeTime = 0.75f;

        private const int
            CryingMax = 2,
            SadMax = 4,
            NeutralMax = 7,
            HappyMax = 9;

        private void Awake()
        {
            _movement = GetComponent<FishNormalMovement>();
            _movement.MovementDone.AddListener(_movement.Wander);
        }

        public void Init(TankFish fish)
        {
            Fish = fish;
            
            name = $"{fish.Fish.Species.Name} ({fish.Fish.Weight:F1} KG)";
            visuals.Init(fish.Fish.Species.Icon, fish.Fish.Size);
            
            _movement.Init(fish.Fish.Species.Speed, fish.Fish.Size);
            _movement.Wander();

            Fish.HappinessChanged.AddListener(OnHappinessChanged);

            UpdateHappinessImage(false);
        }

        private void LateUpdate()
        {
            bool shouldBeVisible = _hovering || _showingHappinessChange;

            if (happinessImage.gameObject.activeSelf != shouldBeVisible)
                happinessImage.gameObject.SetActive(shouldBeVisible);

            if (_hovering)
                SetHappinessImageAlpha(1f);

            _hovering = false;
        }

        private void OnHappinessChanged() => UpdateHappinessImage(true);
        
        private void UpdateHappinessImage(bool showChange)
        {
            Sprite newSprite = Fish.Happiness switch
            {
                <= CryingMax => cryingSprite,
                <= SadMax => sadSprite,
                <= NeutralMax => neutralSprite,
                <= HappyMax => happySprite,
                _ => loveSprite
            };
            
            if (happinessImage.sprite == newSprite)
                return;

            happinessImage.sprite = newSprite;

            if (showChange)
                ShowHappinessImage();
        }

        private void ShowHappinessImage()
        {
            if (_happinessImageRoutine != null)
                StopCoroutine(_happinessImageRoutine);

            _happinessImageRoutine = StartCoroutine(HappinessImageRoutine());
        }

        private IEnumerator HappinessImageRoutine()
        {
            _showingHappinessChange = true;
            happinessImage.gameObject.SetActive(true);
            SetHappinessImageAlpha(1f);

            // Keep the image fully visible for a moment.
            yield return new WaitForSeconds(happinessDisplayTime);

            // Slowly fade it out.
            float elapsed = 0f;

            while (elapsed < happinessFadeTime)
            {
                // Keep it fully visible while the fish is being hovered.
                if (_hovering)
                {
                    SetHappinessImageAlpha(1f);
                    yield return null;
                    continue;
                }

                elapsed += Time.deltaTime;

                float alpha = 1f - Mathf.Clamp01(elapsed / happinessFadeTime);
                SetHappinessImageAlpha(alpha);

                yield return null;
            }

            SetHappinessImageAlpha(0f);
            _showingHappinessChange = false;
            _happinessImageRoutine = null;
        }

        private void SetHappinessImageAlpha(float alpha)
        {
            Color color = happinessImage.color;
            color.a = alpha;
            happinessImage.color = color;
        }

        public void Hovering() => _hovering = true;

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;

        private void OnDestroy() => 
            Fish?.HappinessChanged.RemoveListener(OnHappinessChanged);
    }
}