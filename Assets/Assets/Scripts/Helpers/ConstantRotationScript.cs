using UnityEngine;

public class ConstantRotationScript : MonoBehaviour
{

    Quaternion fixedRotation;

    void Start()
    {
        fixedRotation = gameObject.transform.rotation;
    }

    void LateUpdate()
    {
        gameObject.transform.rotation = fixedRotation;
    }
}
