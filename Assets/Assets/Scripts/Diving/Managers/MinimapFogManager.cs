
using UnityEngine;

public class MinimapFogManager : MonoBehaviour
{
    [Header("Exploration Texture")]
    [SerializeField] private RenderTexture explorationTexture;

    private Texture2D initializationTexture;

    private void Start()
    {
        InitializeExplorationTexture();
    }

    private void InitializeExplorationTexture()
    {
        // Create a temporary white texture.
        initializationTexture = new Texture2D(
            explorationTexture.width,
            explorationTexture.height,
            TextureFormat.R8,
            false
        );

        // Fill the texture with white pixels (unexplored).
        Color[] pixels = new Color[explorationTexture.width * explorationTexture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        initializationTexture.SetPixels(pixels);
        initializationTexture.Apply();

        // Copy the white texture into our RenderTexture.
        Graphics.Blit(initializationTexture, explorationTexture);

        // Clean up the temporary texture.
        Destroy(initializationTexture);
        initializationTexture = null;
    }
}
