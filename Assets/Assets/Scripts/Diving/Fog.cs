using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isFading;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            spriteRenderer.color = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        gameObject.SetActive(false);
    }
}