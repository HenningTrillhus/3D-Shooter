using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }



    public void TakeDamage(float amount, BodyPart bodyPart)
    {
        currentHealth -= amount;
        Debug.Log($"Traff {bodyPart}, tok {amount} skade. HP igjen: {currentHealth}");
        if (bodyPart == BodyPart.Head)
        {
            animator.SetTrigger("HitHead");
        }
        if (bodyPart == BodyPart.Torso)
        {
            animator.SetTrigger("HitChest");
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " døde");
        animator.SetTrigger("Die");
        // her kan vi senere legge til dødsanimasjon, loot, effekter osv.
        //Destroy(gameObject, 3f); // forsinket ødeleggelse for å la dødsanimasjonen spille ferdig
    }
}