using Assets.Scripts.Audio;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
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
        if (col.gameObject.layer != 7 || grabbing || DivingController.Instance.state == PlayerState.STUN) return;
        
        Fish fish = col.gameObject.GetComponent<FishController>().Fish;
        FishCaughtMovement movement = col.gameObject.GetComponent<FishCaughtMovement>();

        FishCollectible fishCol = new(fish);
        if (!DivingInventory.Instance.CanAdd(fishCol))
        {
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            return;
        }
        

        col.gameObject.GetComponent<FishController>().Catch();
        CatchingMinigame.Instance.Init(fish, movement);
        DivingController.Instance.SetPlayerState(PlayerState.GRAB);
        grabbing = true;

        // Stop swimming
        // AudioManager.Instance.FadeOutLoop();
    }

    private void SetGrabbing(bool won)
    {
        AudioManager.Instance.Play(AudioId.CatchFishSuccess);
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
