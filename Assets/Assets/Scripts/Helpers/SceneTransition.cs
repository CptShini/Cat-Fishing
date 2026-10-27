using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    private Animator fadeAnim;
    private Image image;
    public float fadeTime = 3f;

    private void Awake()
    {
        fadeAnim = gameObject.transform.GetChild(0).GetComponent<Animator>(); 
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
        
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
        
        FadeOut();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        fadeAnim.Play("FadeIn");

        yield return new WaitForSeconds(fadeTime);

        SceneManager.LoadScene(sceneName);
        fadeAnim.SetBool("Loaded",true);
    }

    private void FadeOut()
    {
        fadeAnim.Play("FadeOut");
    }

}
