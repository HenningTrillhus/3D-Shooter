using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float turnSpeed = 5f;
    public float waitTimeAtPoint = 2f;
    public float arriveDistance = 0.2f;

    private int currentPointIndex;
    private float waitTimer;
    private bool isWaiting;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (patrolPoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;

            SetWalking(false); // står stille mens den venter
            return;
        }

        Transform target = patrolPoints[currentPointIndex];
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.magnitude <= arriveDistance)
        {
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

            SetWalking(false);
            return;
        }

        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        SetWalking(true); // beveger seg faktisk
    }

    void SetWalking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsWalking", value);
    }
}