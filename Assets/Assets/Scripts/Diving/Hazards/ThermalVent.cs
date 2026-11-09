using UnityEngine;

public class ThermalVent : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public int power;

    private Collider2D _collider;
    [SerializeField] private ParticleSystem _ps;
    [SerializeField] private SpriteRenderer _warningSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private float activeDuration;
    [SerializeField] private float offset = 0;
    [SerializeField] private bool isActive = false;
    private float curTimer;

    void Start()
    {
        _collider = gameObject.GetComponent<Collider2D>();
        curTimer = offset;

        animator.SetFloat("speed", 1 / cooldownDuration);

        SetActive(isActive);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damaged(damage, new Vector2(gameObject.transform.up.x, gameObject.transform.up.y) * power);
            AudioManager.Instance.PlayHurt();
        }
    }

    void SetActive(bool isActive)
    {
        curTimer = 0;

        if (isActive)
        {
            animator.SetBool("exit", false);
            this.isActive = true;
            _collider.enabled = true;
            var emission = _ps.emission;
            emission.enabled = true;
        }
        else
        {
            animator.SetBool("exit", true);
            this.isActive = false;
            _collider.enabled = false;
            var emission = _ps.emission;
            emission.enabled = false;
            animator.Play("WarningClip");
        }
    }

    void Update()
    {
        if (isActive)
        {
            curTimer += Time.deltaTime;
            if(curTimer < activeDuration)
            {
                return;
            }
            SetActive(false);
        }
        else
        {
            curTimer += Time.deltaTime;
            if(curTimer < cooldownDuration)
            {
                return;
            }
            SetActive(true);
        }
    }
}
