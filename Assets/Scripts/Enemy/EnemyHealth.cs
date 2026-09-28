using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }



    public void TakeDamage(float amount, BodyPart bodyPart)
    {
        currentHealth -= amount;
        Debug.Log($"Traff {bodyPart}, tok {amount} skade. HP igjen: {currentHealth}");

        if (bodyPart == BodyPart.Head || bodyPart == BodyPart.Neck)
        {
            animator.SetTrigger("HitHead");
        }
        if (bodyPart == BodyPart.Chest || bodyPart == BodyPart.Heart || bodyPart == BodyPart.Stomach)
        {
            animator.SetTrigger("HitChest");
        }
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
            controller.EnterUnderAttack();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " døde");
        animator.SetTrigger("Die");

        EnemyPatrol patrol = GetComponent<EnemyPatrol>();
        if (patrol != null)
            patrol.enabled = false;

        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
            controller.enabled = false;

        // Destroy(gameObject); // fjernet - enemyen skal bli liggende, ikke forsvinne
    }
}