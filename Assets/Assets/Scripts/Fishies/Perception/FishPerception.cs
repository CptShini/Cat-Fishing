using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fishies.Perception
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class FishPerception : MonoBehaviour
    {
        private protected abstract string TriggerTag();
        private protected abstract bool IsOffset();
        private protected abstract float ComputeReactionTime(int reactionStat);
        private protected abstract float ComputeRadius(int sizeStat);

        private float _reactionTime;
        private float _radius;
        
        public bool Detecting { get; private set; }
        public bool Cooldown { get; private set; }
        public float ReactionPercent => _reactionMeter / _reactionTime;
        
        private float _reactionMeter;

        private Collider2D _other;
        private ColliderDistance2D _colDistance;
        
        private protected string _triggerTag;
        private CircleCollider2D _col;
        
        private void Awake()
        {
            _triggerTag = TriggerTag();
            _col = GetComponent<CircleCollider2D>();
            Init(2, 2);
        }

        public void Init(int reactionStat, int sizeStat)
        {
            if (sizeStat == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _reactionTime = ComputeReactionTime(reactionStat);
            _radius = ComputeRadius(sizeStat);
            
            _col.radius = _radius;
            _col.offset = IsOffset() ? new(_radius * 0.8f, 0) : Vector2.zero;
        }

        private void Update()
        {
            if (!Detecting || Cooldown)
            {
                if (_reactionMeter > 0f)
                    _reactionMeter -= Time.deltaTime;
                else
                {
                    _reactionMeter = 0f;
                    Cooldown = false;
                }

                return;
            }
            
            float closenessFactor = 1f + -_colDistance.distance / _radius;
            _reactionMeter += Time.deltaTime * closenessFactor;
            if (_reactionMeter < _reactionTime) return;
            
            Cooldown = true;
            Detect(_other);
        }

        private protected abstract void Detect(Collider2D other);
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (Cooldown || !other.CompareTag(_triggerTag))
                return;
            
            Detecting = PerceptionCheck(other);
            if (!Detecting)
                return;

            _other = other;
            _colDistance = other.Distance(_col);
        }
        
        private protected abstract bool PerceptionCheck(Collider2D other);

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(_triggerTag))
                return;
            
            Detecting = false;
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
                Gizmos.color = Cooldown 
                    ? Color.blueViolet 
                    : Detecting ? Color.hotPink : Color.white;
                float r = ReactionPercent * _col.radius;
                Gizmos.DrawSphere(_col.bounds.center, r);
            }
        }
    }
}