using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Referanser")]
    public Transform player;
    public Transform eyes; // valgfritt: tomt objekt ved hodet, ellers brukes enemyens posisjon

    [Header("Synsfelt")]
    public float viewDistance = 10f;
    [Range(1f, 180f)]
    public float viewAngle = 90f; // total vinkel, altså 45° til hver side
    public LayerMask obstacleMask; // vegger o.l. som blokkerer sikten (IKKE spilleren/hitboxer)

    [Header("Deteksjon (0-100)")]
    public float increaseSpeed = 40f;      // poeng per sekund når spilleren er synlig
    public float decreaseSpeed = 15f;      // poeng per sekund når spilleren ikke er synlig
    [Range(0f, 1f)]
    public float farDistanceFactor = 0.3f; // hvor sakte det øker helt ytterst i feltet (1 = lik fart overalt)

    public float Detection { get; private set; }
    public bool HasSeenPlayer => Detection >= 100f;
    public bool PlayerInView { get; private set; }

    void Start()
    {
        if (player == null)
        {
            PlayerAim pa = FindFirstObjectByType<PlayerAim>();
            if (pa != null) player = pa.transform;
        }

        if (eyes == null) eyes = transform;
    }

    void Update()
    {
        if (player == null) return;

        PlayerInView = CanSeePlayer(out float distance);

        if (PlayerInView)
        {
            // Nærmere spiller = raskere økning
            float t = Mathf.Clamp01(distance / viewDistance);
            float distanceMultiplier = Mathf.Lerp(1f, farDistanceFactor, t);

            Detection += increaseSpeed * distanceMultiplier * Time.deltaTime;
        }
        else
        {
            Detection -= decreaseSpeed * Time.deltaTime;
        }

        Detection = Mathf.Clamp(Detection, 0f, 100f);
        Debug.Log("Detection: " + Detection + ", PlayerInView: " + PlayerInView + ", HasSeenPlayer: " + HasSeenPlayer);
    }

    bool CanSeePlayer(out float distance)
    {
        Vector3 eyePos = eyes.position;
        Vector3 targetPos = player.position + Vector3.up * 1f; // sikt mot brystet, ikke føttene

        Vector3 toPlayer = targetPos - eyePos;
        distance = toPlayer.magnitude;

        // 1. Innenfor rekkevidde?
        if (distance > viewDistance) return false;

        // 2. Innenfor vinkelen foran enemyen? (kun horisontalt)
        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        Vector3 flatForward = new Vector3(eyes.forward.x, 0f, eyes.forward.z);

        if (Vector3.Angle(flatForward, flatToPlayer) > viewAngle / 2f) return false;

        // 3. Ingen vegg mellom?
        if (Physics.Linecast(eyePos, targetPos, obstacleMask)) return false;

        return true;
    }

    // Tegner synsfeltet i Scene-vinduet
    void OnDrawGizmosSelected()
    {
        Transform e = eyes != null ? eyes : transform;

        Gizmos.color = PlayerInView ? Color.red : Color.yellow;

        Vector3 left = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.up) * e.forward;
        Vector3 right = Quaternion.AngleAxis(viewAngle / 2f, Vector3.up) * e.forward;

        Gizmos.DrawRay(e.position, left * viewDistance);
        Gizmos.DrawRay(e.position, right * viewDistance);
        Gizmos.DrawWireSphere(e.position, 0.1f);
    }
}