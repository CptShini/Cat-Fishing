using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DivingInventorySlot : MonoBehaviour
{

    [SerializeReference] public Collectible collectible;
    Button button;

    Image image;

    TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
        text = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { DivingInventory.Instance.Drag(this); });
        
        text.text = Mathf.RoundToInt(collectible.weight) + " kg";
    }

    public void Setup(Collectible collectible)
    {
        this.collectible = collectible;
        Debug.Log($"{collectible.sprite.ToString()}");
        Debug.Log("HELLO? " + image.ToString());
        image.sprite = collectible.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
