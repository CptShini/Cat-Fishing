using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Upgrades;
using Assets.Scripts.Data.Persistence;

public class Fog : MonoBehaviour, IDataPersistence
{
    private static bool saved, loaded, hasMinimap;
    private static HashSet<int> exploredFogs = new();

    [SerializeField] private int fogID;

    private bool _isFading;
    private SpriteRenderer _spriteRenderer;
    
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = true;
        
        _collider = GetComponent<PolygonCollider2D>();
        _collider.enabled = false;
    }

    private void Start() => gameObject.SetActive(!exploredFogs.Contains(fogID));
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasMinimap || !other.CompareTag("Player") || _isFading) return;
        
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        _isFading = true;

        float duration = 2f;
        float elapsed = 0f;

        Color color = _spriteRenderer.color;
        float startAlpha = color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);

            _spriteRenderer.color = new(color.r, color.g, color.b, alpha);

            yield return null;
        }

        _spriteRenderer.color = new(color.r, color.g, color.b, 0f);

        // Remember explored state
        exploredFogs.Add(fogID);

        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        if (!loaded)
        {
            loaded = true;
            hasMinimap = data.OneTimeUpgrades[OneTimeUpgrade.Minimap];
            exploredFogs = data.ExploredFogs.ToHashSet();
        }
        
        _collider.enabled = true;
    }

    public void SaveData(ref GameData data)
    {
        if (saved) return;
        
        saved = true;
        data.ExploredFogs = exploredFogs.ToList();
    }
}