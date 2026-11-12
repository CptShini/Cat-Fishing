using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AimPawScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> paws;
    
    private Animator animator;
    
    public int activePaws;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AimPawEnabler(bool enable)
    {
        if(enable)
        {
            foreach (GameObject paw in paws)
            {
                paw.SetActive(false);
                activePaws = 0;
            }
            animator.Play("Paw");
            animator.SetFloat("speed", DivingPlayerStats.Instance.chargeUpSpeed / DivingPlayerStats.Instance.maxPounceStamina);
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
