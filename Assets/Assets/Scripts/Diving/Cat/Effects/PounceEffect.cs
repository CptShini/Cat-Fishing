using System.Collections;
using UnityEngine;

public class PounceEffect : MonoBehaviour
{

    private Animator animator;

    private IEnumerator coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        coroutine = Play();
        StartCoroutine(coroutine);
    }

    private IEnumerator Play() {

        animator.Play("Effect");
        Debug.Log("HEELLOOOO??");
        yield return new WaitForSeconds(animator.GetFloat("speed"));
        Debug.Log("Finished??");

        Destroy(gameObject);
    }
}
