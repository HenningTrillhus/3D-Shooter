using UnityEngine;

public enum EnemyState
{
    Patrol,
    UnderAttack
}

public class EnemyController : MonoBehaviour
{
    public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

    private EnemyVision vision;
    private EnemyPatrol patrol;
    private EnemyHealth health;

    [Header("Under Attack")]
    public float attackMemoryTime = 5f;
    private float lastContactTimer;

    [Header("Cover")]
    public Transform[] coverPoints;
    public float moveSpeed = 3f;
    public float turnSpeed = 5f;
    public float arriveDistance = 0.3f;

    private Transform currentCover;
    private Animator animator;

    void Awake()
    {
        vision = GetComponent<EnemyVision>();
        patrol = GetComponent<EnemyPatrol>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (vision != null && vision.HasSeenPlayer)
        {
            EnterUnderAttack();
        }

        if (CurrentState == EnemyState.UnderAttack)
        {
            lastContactTimer -= Time.deltaTime;

            MoveToCover();

            if (lastContactTimer <= 0f)
            {
                ExitUnderAttack();
            }
        }
    }

    public void EnterUnderAttack()
    {
        lastContactTimer = attackMemoryTime;

        if (CurrentState == EnemyState.UnderAttack) return;

        CurrentState = EnemyState.UnderAttack;

        if (patrol != null)
            patrol.enabled = false;

        currentCover = FindNearestCover();

        Debug.Log(gameObject.name + " gikk inn i UnderAttack, søker cover: " + (currentCover != null ? currentCover.name : "ingen"));
    }

    void ExitUnderAttack()
    {
        CurrentState = EnemyState.Patrol;
        currentCover = null;

        if (patrol != null)
            patrol.enabled = true;

        if (animator != null)
            animator.SetBool("IsWalking", false);

        Debug.Log(gameObject.name + " gikk tilbake til Patrol");
    }

    Transform FindNearestCover()
    {
        if (coverPoints.Length == 0) return null;

        Transform nearest = coverPoints[0];
        float nearestDist = Vector3.Distance(transform.position, nearest.position);

        foreach (Transform point in coverPoints)
        {
            float dist = Vector3.Distance(transform.position, point.position);
            if (dist < nearestDist)
            {
                nearest = point;
                nearestDist = dist;
            }
        }

        return nearest;
    }

    void MoveToCover()
    {
        if (currentCover == null)
        {
            if (animator != null) animator.SetBool("IsRunning", false);
            return;
        }

        Vector3 dir = currentCover.position - transform.position;
        dir.y = 0f;

        if (dir.magnitude <= arriveDistance)
        {
            if (animator != null) animator.SetBool("IsRunning", false);
            return;
        }

        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        if (animator != null) animator.SetBool("IsRunning", true);
    }
}