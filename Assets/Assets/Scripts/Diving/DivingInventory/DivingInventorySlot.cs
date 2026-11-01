using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DivingInventorySlot : MonoBehaviour
{
    [SerializeReference] public Collectible collectible;
    Button button;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { DivingInventory.Instance.Drag(this); });
        
        text.text = Mathf.RoundToInt(collectible.weight) + " kg";
    }

    public void Setup(Collectible collectible)
    {
        this.collectible = collectible;
        image.sprite = collectible.sprite;
    }
}