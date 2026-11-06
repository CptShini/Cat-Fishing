using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;


public class CatchScript : MonoBehaviour
{
    bool grabbing = false;
    Vector2 velocityVector = new Vector2(0,0);
    private Rigidbody2D _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        CatchingMinigame.Instance.OnEnded.AddListener(SetGrabbing);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != 7 || grabbing) return;
        
        Fish fish = col.gameObject.GetComponent<FishStats>().fish;
        FishCaughtMovement movement = col.gameObject.GetComponent<FishCaughtMovement>();

        col.gameObject.GetComponent<FishController>().Catch();
        CatchingMinigame.Instance.Init(fish, movement);
        DivingController.Instance.SetPlayerState(PlayerState.GRAB);
        grabbing = true;
    }

    private void SetGrabbing(bool won)
    {
        grabbing = false;
        
    }

    public void OnGrabbing(CallbackContext input)
    {
        velocityVector = input.ReadValue<Vector2>();
    }

    private void Grab()
    {
        _rb.linearVelocity = new Vector3(0, 0, 0);
        CatchingMinigame.Instance.Grab(velocityVector);
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbing)
        {
            Grab();
        }
    }
}
