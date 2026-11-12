using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnim;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI info;

        private static readonly int Loaded = Animator.StringToHash("Loaded");

        public void FadeOut(string text)
        {
            info.text = text;
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
