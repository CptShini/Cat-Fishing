using UnityEngine;

public class ThermalVent : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public int power;

    private Collider2D _collider;
    private SpriteRenderer _sprite;

    [SerializeField] private float cooldownDuration;
    [SerializeField] private float activeDuration;

    private float curTimer = 0;
    private bool isActive = false;


    void Start()
    {
        _collider = gameObject.GetComponent<Collider2D>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();

        SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damaged(damage, new Vector2(gameObject.transform.up.x, gameObject.transform.up.y) * power);
        }
    }

    void SetActive(bool isActive)
    {
        curTimer = 0;

        if (isActive)
        {
            this.isActive = true;
            _collider.enabled = true;
            _sprite.enabled = true;
        }
        else
        {
            this.isActive = false;
            _collider.enabled = false;
            _sprite.enabled = false;
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
