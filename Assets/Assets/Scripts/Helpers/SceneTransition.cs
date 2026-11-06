using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [SerializeField] private float fadeTime = 3f;
    
        [SerializeField] private Animator fadeAnim;
        [SerializeField] private Image image;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        
            FadeOut();
        }

        private void Start() => 
            MusicPlayer.Instance.PlayTrack(
                SceneManager.GetActiveScene().buildIndex, 
                fadeTime
            );

        public void LoadScene(int buildIndex) => 
            StartCoroutine(LoadSceneWithFade(buildIndex));

        private IEnumerator LoadSceneWithFade(int buildIndex)
        {
            MusicPlayer.Instance.StopTrack(fadeTime);
            fadeAnim.SetBool("Loaded", false); // TODO: shouldn't be done with strings :/
            fadeAnim.Play("FadeIn");

            yield return new WaitForSeconds(fadeTime);

            SceneManager.LoadScene(buildIndex);
            fadeAnim.SetBool("Loaded", true);
            MusicPlayer.Instance.PlayTrack(buildIndex, fadeTime);
        }

        private void FadeOut()
        {
            fadeAnim.Play("FadeOut");
        }

    }
}
