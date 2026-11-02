using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fish.Perception
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class FishPerception : MonoBehaviour
    {
        [SerializeField] private int reactionTimeMs = 1000;
        [SerializeField] private float radius = 1f;

        private protected abstract string TriggerTag();
        private protected abstract bool IsOffset();

        private int _reactionTimeMs;
        private float _radius;
        
        private bool _detecting, _cooldown;
        private float _reactionMeter;

        private Collider2D _other;
        private ColliderDistance2D _colDistance;
        
        private string _triggerTag;
        private CircleCollider2D _col;
        
        private void Start()
        {
            _triggerTag = TriggerTag();
            _col = GetComponent<CircleCollider2D>();
            if (!_col.isTrigger) Debug.LogWarning($"This ({name}) needs to be isTrigger!");
            
            _col.radius = radius;
            _col.offset = IsOffset() ? new(radius * 0.8f, 0) : Vector2.zero;
        }

        public void Init(int reactionStat, int sizeStat)
        {
            _reactionTimeMs = reactionStat * -490 + 5000;
            //_radius = 
        }
        
        private protected abstract float RadiusFromSizeStat(int sizeStat);

        private void Update()
        {
            if (!_detecting || _cooldown)
            {
                if (_reactionMeter > 0f)
                    _reactionMeter -= Time.deltaTime;
                else
                {
                    _reactionMeter = 0f;
                    _cooldown = false;
                }

                return;
            }
            
            float closenessFactor = 1f + -_colDistance.distance / radius;
            _reactionMeter += Time.deltaTime * closenessFactor;
            if (_reactionMeter < reactionTimeMs / 1000f) return;
            
            _cooldown = true;
            Detect(_other);
        }

        private protected abstract void Detect(Collider2D other);
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(_triggerTag))
                return;
            
            _detecting = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_cooldown || !other.CompareTag(_triggerTag))
                return;

            _other = other;
            _colDistance = other.Distance(_col);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(_triggerTag))
                return;
            
            _detecting = false;
        }

        private void OnDrawGizmos()
        {
            if (DebugDrawer.Instance == null || !DebugDrawer.Instance.EnableDebug
                || !DebugDrawer.Instance.Fish_Perception) return;
            
            if (DebugDrawer.Instance.Fish_Perception_DrawReactionTime) DrawReactionTime();
            if (DebugDrawer.Instance.Fish_Perception_DrawCircle) DrawCircle();

            return;

            void DrawCircle()
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(_col.bounds.center, _col.radius);
            }

            void DrawReactionTime()
            {
                Gizmos.color = _cooldown 
                    ? Color.blueViolet 
                    : _detecting ? Color.hotPink : Color.white;
                float r = _reactionMeter / (reactionTimeMs / 1000f) * _col.radius;
                Gizmos.DrawSphere(_col.bounds.center, r);
            }
        }
    }
}