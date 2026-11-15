using Assets.Scripts.Helpers;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    private Animator animator;
    private Vector2 curVelocity;
    private NavMeshAgent navMeshAgent;
    private NavMeshSurface surface;

    [SerializeField] private float maxWalkDistance = 10f;
    [SerializeField] private float centerWeight = 0.3f;
    [SerializeField] private Range destinationCooldownRange;

    private float curDestinationCooldown;
    private float curCooldown = float.MaxValue;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        surface = NavigationManager.Instance.surface;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void SetNewTargetPosition()
    {
        Vector2 direction = Random.insideUnitCircle * maxWalkDistance;
        direction += (Vector2)transform.position;

        Vector2 weightedDirection = Vector2.Lerp(direction, surface.center, centerWeight);
        NavMeshQueryFilter filter = new()
        {
            agentTypeID = navMeshAgent.agentTypeID,
            areaMask = navMeshAgent.areaMask
        };

        if (!NavMesh.SamplePosition(weightedDirection, out NavMeshHit hit, maxWalkDistance, filter))
        {
            Debug.LogWarning($"[{name}] Could not find a valid NavMesh position. Position: {transform.position}, Search position: {weightedDirection}");
            return;
        }

        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[{name}] Agent is not on a NavMesh!");
            return;
        }

        if (!navMeshAgent.SetDestination(hit.position))
        {
            Debug.LogWarning($"[{name}] Failed to set destination: {hit.position}");
            return;
        }

        curCooldown = 0;
        curDestinationCooldown = destinationCooldownRange.Random();
    }

    private void FixedUpdate()
    {
        Move(navMeshAgent.velocity);
        
        curVelocity = navMeshAgent.velocity;
        if (curCooldown > curDestinationCooldown)
            SetNewTargetPosition();
        else
            curCooldown += Time.deltaTime;

        bool isWalking = curVelocity != Vector2.zero;
        animator.SetBool("isWalking", isWalking);
        /*if (isWalking)
            AudioManager.Instance.Play(AudioId.Walk);
        else
            AudioManager.Instance.FadeOutLoop(AudioId.Walk);*/
    }

    private void Move(Vector2 newDirection)
    {
        Vector2 curVelocityNorm = newDirection.normalized;
        if (curVelocity == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", curVelocityNorm.x);
            animator.SetFloat("LastInputY", curVelocityNorm.y);
        }

        animator.SetFloat("InputX", curVelocityNorm.x);
        animator.SetFloat("InputY", curVelocityNorm.y);
    }

}