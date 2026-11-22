

using Assets.Scripts.Audio;
using Assets.Scripts.Audio.SFX;
using UnityEngine;

namespace Assets.Scripts.Diving.Hazards
{
    public class Stream : EnvironmentalMovementAffector
    {

        void Start()
        {
            _direction = new Vector2(gameObject.transform.up.x, gameObject.transform.up.y);
        }

    }
}
