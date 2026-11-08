using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnim;
        [SerializeField] private Image image;

        private static readonly int Loaded = Animator.StringToHash("Loaded");

        public void FadeOut()
        {
            fadeAnim.SetBool(Loaded, false);
            fadeAnim.Play("FadeIn"); // Yeah names are wrong, whatever
        }

        public void FadeIn()
        {
            fadeAnim.Play("FadeOut"); // Yeah names are wrong, whatever
            fadeAnim.SetBool(Loaded, true);
        }
    }
}
