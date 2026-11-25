using System.Collections.Generic;
using UnityEngine;

public class DamagedEffect : MonoBehaviour
{

    [SerializeField] private float _decaySpeed;
    [SerializeField] private List<SpriteRenderer> effectedSprites;
    private MaterialPropertyBlock _propertyBlock;

    private float _factor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<DamagedScript>().OnDamaged.AddListener(Blink);
        _propertyBlock = new();
    }

    public void Blink()
    {
        _factor = 1f;
        ApplyBlinkFactor();
    }

    private void ApplyBlinkFactor()
    {
        foreach (var renderer in effectedSprites)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_BlinkFactor", _factor);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_factor <= 0f) return;

        _factor = Mathf.Lerp(_factor, 0f, Time.deltaTime * _decaySpeed);

        if(_factor < 0.01f)
        {
            _factor = 0;
        }

        ApplyBlinkFactor();
    }
}
