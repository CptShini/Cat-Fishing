using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies;
using Assets.Scripts.PlayerState;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CatchScript : MonoBehaviour
{
    private bool _grabbing;
    private Vector2 _velocityVector = new(0,0);
    private Rigidbody2D _rb;

    
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        CatchingMinigame.Instance.OnEnded.AddListener(SetGrabbing);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != 7 || _grabbing || DivingController.Instance.state != PlayerState.POUNCE) return;
        
        FishController fc = col.gameObject.GetComponent<FishController>();
        FishCollectible fishCol = new(fc.Fish);
        if (!DivingInventory.Instance.CanAdd(fishCol))
        {
            DivingController.Instance.SetPlayerState(PlayerState.STUN);
            return;
        }

        fc.Catch();
        CatchingMinigame.Instance.Init(fc.Fish, fc.CaughtMovement);
        DivingController.Instance.SetPlayerState(PlayerState.GRAB);
        _grabbing = true;

        // Stop swimming
        // AudioManager.Instance.FadeOutLoop();
    }

    private void SetGrabbing(bool won)
    {
        SFXManager.Instance.Play(SFXId.CatchFishSuccess);
        _grabbing = false;
    }

    public void OnGrabbing(CallbackContext input)
    {
        _velocityVector = input.ReadValue<Vector2>();
    }

    private void Grab()
    {
        _rb.linearVelocity = new Vector3(0, 0, 0);
        CatchingMinigame.Instance.Grab(_velocityVector);
    }
    
    private void Update()
    {
        if (!_grabbing) return;
        
        Grab();
    }
}
