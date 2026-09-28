using UnityEngine;

public class EnemyAimIndicator : MonoBehaviour
{
    public PlayerAim playerAim;       // dra inn spilleren, eller la den finnes automatisk
    public GameObject indicator;      // objektet under enemyen som skal vises
    public float radius = 1.5f;       // hvor nær siktepunktet må være

    void Start()
    {
        if (playerAim == null)
            playerAim = FindFirstObjectByType<PlayerAim>();

        if (indicator != null)
            indicator.SetActive(false);
    }

    void Update()
    {
        if (playerAim == null || indicator == null) return;

        bool show = false;

        if (playerAim.isAiming)
        {
            Vector3 aim = playerAim.AimPoint;
            Vector3 self = transform.position;

            aim.y = 0f;
            self.y = 0f;

            show = Vector3.Distance(aim, self) <= radius;
        }

        if (indicator.activeSelf != show)
            indicator.SetActive(show);
    }

    // Viser radiusen i Scene-vinduet så du kan justere den visuelt
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}