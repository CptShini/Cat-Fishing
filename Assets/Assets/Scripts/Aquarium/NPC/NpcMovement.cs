using Assets.Scripts.Audio;
using Assets.Scripts.Helpers;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NpcMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 curVelocity;
    private NavMeshAgent navMeshAgent;

    private NavMeshSurface surface;
    private CapsuleCollider2D playerCollider;
    public Tilemap groundTilemap;

    [SerializeField] private float maxWalkDistance = 10f;

    [SerializeField] private Range destinationCooldownRange = new();

    private float curDestinationCooldown;
    private float curCooldown = float.MaxValue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        surface = NavigationManager.Instance.surface;
    }

    private void SetDestionationCooldown()
    {
        curDestinationCooldown = destinationCooldownRange.Random();
    }

    void SetNewTargetPosition()
    {
        Vector2 direction = Random.insideUnitCircle * maxWalkDistance;
        direction = new Vector2(direction.x + transform.position.x, direction.y + transform.position.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(direction, out hit, maxWalkDistance, 1);
		
        Vector3 destination = hit.position;
        
        navMeshAgent.SetDestination(destination);
        curCooldown = 0;
        SetDestionationCooldown();
    }

    void FixedUpdate()
    {
        if(PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            AudioManager.Instance.FadeOutLoop(AudioId.Walk);
            return;
        }

        //navMeshAgent.destination = targetTransform.position;
        if (curVelocity.x != navMeshAgent.velocity.x || curVelocity.y != navMeshAgent.velocity.y)
        {
            Move(navMeshAgent.velocity);
        }

        curVelocity = navMeshAgent.velocity;        

        if(curCooldown > curDestinationCooldown)
        {
            SetNewTargetPosition();
        }
        else
        {
            curCooldown += Time.deltaTime;
        }

        bool isWalking = curVelocity != Vector2.zero;
        animator.SetBool("isWalking", isWalking);
        if (isWalking)
            AudioManager.Instance.Play(AudioId.Walk);
        else
            AudioManager.Instance.FadeOutLoop(AudioId.Walk);
    }

    public void Move(Vector2 newDirection)
    {
        Vector2 curVelocityNorm = newDirection.normalized;
        
        if(curVelocity == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", curVelocityNorm.x);
            animator.SetFloat("LastInputY", curVelocityNorm.y);
        }

        animator.SetFloat("InputX", curVelocityNorm.x);
        animator.SetFloat("InputY", curVelocityNorm.y);
    }

}