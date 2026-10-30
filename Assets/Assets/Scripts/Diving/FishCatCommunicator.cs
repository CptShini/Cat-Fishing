using Assets.Scripts.Fish;
using UnityEngine;

public class FishCatCommunicator : MonoBehaviour
{
    [SerializeField] public FishSO fishSO;

    public Fish fish;
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

        fish = new Fish(fishSO);
        Debug.Log($"FISH NAME: {fish.fishName}");
    }

    // Update is called once per frame
    void Update()
    {
        body.linearVelocity = direction;
    }
}
