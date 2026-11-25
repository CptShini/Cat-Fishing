namespace Assets.Scripts.Diving.Hazards
{
    public class Stream : EnvironmentalMovementAffector
    {
        private void Start() => 
            _direction = transform.right;
    }
}
