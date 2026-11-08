using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnim;
        [SerializeField] private Image image;

        private static readonly int Loaded = Animator.StringToHash("Loaded");
        
        private void Awake() => FadeOut();

        public void FadeIn()
        {
            fadeAnim.SetBool(Loaded, false);
            fadeAnim.Play("FadeIn");
        }

        public void FadeOut()
        {
            fadeAnim.Play("FadeOut");
            fadeAnim.SetBool(Loaded, true);
        }
    }
}
