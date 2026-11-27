using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Upgrades;
using Assets.Scripts.Data.Persistence;

public class Fog : MonoBehaviour, IDataPersistence
{
    private static bool hasMinimap;
    private static HashSet<int> exploredFogs = new();

    [SerializeField] private int fogID;

    private bool _isFading;

    private SpriteRenderer _spriteRenderer;
    private PolygonCollider2D _collider;

    private Color _originalColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<PolygonCollider2D>();

        _originalColor = _spriteRenderer.color;
        
        _collider.isTrigger = true;
        _collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) => 
        TryReveal(other);

    private void OnTriggerStay2D(Collider2D other) => 
        TryReveal(other);

    private void TryReveal(Collider2D other)
    {
        if (!hasMinimap || !other.CompareTag("Player") || _isFading)
            return;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        _isFading = true;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                _originalColor.a,
                0f,
                elapsed / duration
            );

            _spriteRenderer.color = new(
                _originalColor.r,
                _originalColor.g,
                _originalColor.b,
                alpha
            );

            yield return null;
        }

        // Remember that this fog has been explored.
        exploredFogs.Add(fogID);

        // Hide the fog without disabling the GameObject.
        HideFog();
    }

    private void HideFog()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    private void ShowFog()
    {
        _spriteRenderer.color = _originalColor;

        _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }

    public void LoadData(GameData data)
    {
        hasMinimap = data.OneTimeUpgrades[OneTimeUpgrade.Minimap];

        exploredFogs = data.ExploredFogs?.ToHashSet() ?? new HashSet<int>();

        if (exploredFogs.Contains(fogID))
            HideFog();
        else
            ShowFog();
    }

    public void SaveData(ref GameData data) => 
        data.ExploredFogs = exploredFogs.ToList();
}