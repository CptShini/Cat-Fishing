using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fog : MonoBehaviour
{
    private static HashSet<string> exploredFogs = new HashSet<string>();

    [SerializeField] private string fogID;

    private SpriteRenderer spriteRenderer;
    private bool isFading;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if this fog has already been explored
        if (exploredFogs.Contains(fogID))
        {
            gameObject.SetActive(false);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetExploredFogs()
    {
        exploredFogs.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            Debug.Log("Player entered the fog");
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        float duration = 2f;
        float elapsed = 0f;

        Color color = spriteRenderer.color;
        float startAlpha = color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);

            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        // Remember explored state
        exploredFogs.Add(fogID);

        gameObject.SetActive(false);
    }
}