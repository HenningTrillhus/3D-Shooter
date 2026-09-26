using UnityEngine;

public class AimSpread : MonoBehaviour
{
    private PlayerAim playerAim;
    private PlayerMovement playerMovement;

    [Header("Hip-fire (ikke sikter)")]
    public float hipFireSpread = 20f;

    [Header("Sikter - start og etter en stund stillestående")]
    public float aimSpreadMax = 10f; // rett etter du begynner å sikte
    public float aimSpreadMin = 2f;  // etter å ha stått stille og siktet en stund
    public float aimInTime = 1.2f;   // sekunder det tar å gå fra max til min

    [Header("Modifikatorer")]
    public float movingSpreadAddition = 8f;
    public float crouchSpreadMultiplier = 0.6f;

    [Header("Overgang")]
    public float spreadChangeSpeed = 25f; // grader per sekund, hvor fort currentSpread følger target

    public float CurrentSpread { get; private set; }

    private float aimTimer = 0f;

    [Header("Recoil (skyting øker spredningen midlertidig)")]
    public float recoilPerShot = 4f;    // hvor mye spread øker per skudd
    public float recoilMax = 15f;       // tak på hvor mye recoil kan bygge seg opp
    public float recoilDecaySpeed = 10f; // hvor fort recoil-bidraget avtar igjen

    private float currentRecoil = 0f;


    void Awake()
    {
        playerAim = GetComponent<PlayerAim>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        float targetSpread;

        if (!playerAim.isAiming)
        {
            targetSpread = hipFireSpread;
            aimTimer = 0f;

            if (playerMovement.IsMoving)
                targetSpread += movingSpreadAddition;
        }
        else
        {
            if (!playerMovement.IsMoving)
                aimTimer += Time.deltaTime;
            else
                aimTimer = 0f;

            float t = Mathf.Clamp01(aimTimer / aimInTime);
            targetSpread = Mathf.Lerp(aimSpreadMax, aimSpreadMin, t);

            if (playerMovement.IsMoving)
                targetSpread += movingSpreadAddition * 0.5f;

            if (playerMovement.IsCrouching)
                targetSpread *= crouchSpreadMultiplier;
        }

        targetSpread += currentRecoil; // <-- nytt: legg til recoil-bidraget

        CurrentSpread = Mathf.MoveTowards(CurrentSpread, targetSpread, spreadChangeSpeed * Time.deltaTime);

        currentRecoil = Mathf.MoveTowards(currentRecoil, 0f, recoilDecaySpeed * Time.deltaTime); // <-- nytt: recoil avtar
    }

    public void AddRecoil()
    {
        currentRecoil = Mathf.Min(currentRecoil + recoilPerShot, recoilMax);
    }
}