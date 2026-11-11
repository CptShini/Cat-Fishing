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
        yield return new WaitForSeconds(animator.GetFloat("speed"));

        Destroy(gameObject);
    }
}
