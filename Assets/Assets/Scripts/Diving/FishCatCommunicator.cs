using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

public class FishCatCommunicator : MonoBehaviour
{
    [SerializeField] public FishSpecies fishSpecies;

    public FishDTO fishDto;
    public HingeJoint2D joint;
    public Rigidbody2D body;
    public Vector2 direction;
    public float strength;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        joint = gameObject.GetComponent<HingeJoint2D>();
        body = gameObject.GetComponent<Rigidbody2D>();

        fishDto = new FishDTO(fishSpecies);
        Debug.Log($"FISH NAME: {fishDto.Name}");
    }

    // Update is called once per frame
    void Update()
    {
        body.linearVelocity = direction;
    }
}
