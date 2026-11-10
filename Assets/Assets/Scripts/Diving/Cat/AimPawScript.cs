using System.Collections.Generic;
using UnityEngine;

public class AimPawScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> paws;
    private Animator animator;
    public int activePaws = 0;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void AimPawEnabler(bool enable)
    {
        if(enable)
        {
            for (int i = 0; i < paws.Count; i++)
            {
                paws[i].SetActive(false);
                activePaws = 0;
            }
            animator.Play("Paw");
            animator.SetFloat("speed", 1.0f / DivingPlayerStats.Instance.chargeUpSpeed);
            animator.SetBool("exit", false);
        }
        else
        {
            animator.SetBool("exit", true);
        } 
    }
    void Update()
    {
        for (int i = 0; i < activePaws; i++)
        {
            paws[i].SetActive(true);
        }
    }
}
