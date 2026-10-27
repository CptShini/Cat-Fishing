using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    private void Awake()
    {
        fadeAnim = gameObject.transform.GetChild(0).GetComponent<Animator>();   
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
