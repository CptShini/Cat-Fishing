using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AimPawScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> paws;
    
    private Animator pawAnimator;
    [SerializeField] private Animator vignetteAnimator;

    
    public int activePaws;
    
    private void Awake()
    {
        pawAnimator = GetComponent<Animator>();
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
            pawAnimator.Play("Paw");
            pawAnimator.SetFloat("speed", DivingPlayerStats.Instance.chargeUpSpeed / DivingPlayerStats.Instance.maxPounceStamina);
            pawAnimator.SetBool("exit", false);
            
            vignetteAnimator.Play("FadeIn");
            vignetteAnimator.SetBool("exit", false);
        }
        else
        {
            pawAnimator.SetBool("exit", true);
            vignetteAnimator.SetBool("exit", true);
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
