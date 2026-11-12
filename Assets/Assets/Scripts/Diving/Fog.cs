using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class Fog : MonoBehaviour, IDataPersistence
{
    private static bool saved, loaded;
    private static HashSet<int> exploredFogs = new();

    [SerializeField] private int fogID;

    private bool _isFading;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = true;
    }

    private void Start() => gameObject.SetActive(!exploredFogs.Contains(fogID));
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _isFading) return;
        
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
        if (loaded) return;
        
        loaded = true;
        exploredFogs = data.ExploredFogs.ToHashSet();
    }

    public void SaveData(ref GameData data)
    {
        if (saved) return;
        
        saved = true;
        data.ExploredFogs = exploredFogs.ToList();
    }
}