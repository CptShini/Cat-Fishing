namespace Assets.Scripts.Diving.Bars
{
    public class CatchBar : StatBar
    {
        public static CatchBar Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }
    }
}
