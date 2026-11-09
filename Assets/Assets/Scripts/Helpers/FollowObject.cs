using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class FollowObject : MonoBehaviour
    {

        [SerializeField] private GameObject following;

        //private Vector3 offset;
    
        void Start()
        {
            //offset = transform.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = following.gameObject.transform.position;
        }
    }
}
