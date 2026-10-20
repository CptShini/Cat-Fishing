using UnityEngine;


using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

using UnityEngine.Events;


public class DivingController : MonoBehaviour
{


    public Vector2 velocityVector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }


    public void OnMove(CallbackContext  input)
    {
        velocityVector = input.ReadValue<Vector2>();
        Debug.Log(velocityVector.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
