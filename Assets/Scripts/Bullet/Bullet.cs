using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 3f;
    public float baseDamage = 20f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Hitbox hitbox = other.GetComponent<Hitbox>();

        if (hitbox != null)
        {
            float damage = baseDamage * hitbox.damageMultiplier;
            hitbox.ownerHealth.TakeDamage(damage, hitbox.bodyPart);
        }
        else
        {
            Debug.Log("Traff noe uten hitbox: " + other.name);
        }

        Destroy(gameObject);
    }
}