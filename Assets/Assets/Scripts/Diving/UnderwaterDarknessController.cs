
using UnityEngine;
using UnityEngine.UI;

public class UnderwaterDarknessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Image darknessImage;

    [Header("Depth Settings")]
    private float depthTopY = 14f;
    private float depthBottomY = -45f; // slightly below center (Y = -65 bottom edge)

    [Header("Darkness Settings")]
    private float maxDarkness = 0.98f;

    private Material runtimeMaterial;

    private void Start()
    {
        runtimeMaterial = new Material(darknessImage.material);
        darknessImage.material = runtimeMaterial;

        runtimeMaterial.SetVector("_PlayerPosition", new Vector4(0.5f, 0.5f, 0, 0));
    }

    private void Update()
    {
        float darkness = Mathf.InverseLerp(
            depthTopY,
            depthBottomY,
            player.position.y
        );

        if (darkness > maxDarkness)
            darkness = maxDarkness;

        runtimeMaterial.SetFloat("_Darkness", darkness);
        // Debug.Log("Darkness: " + darkness);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
