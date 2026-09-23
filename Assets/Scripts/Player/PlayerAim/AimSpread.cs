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

    void Awake()
    {
        playerAim = GetComponent<PlayerAim>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        //Debug.Log($"Spread: {CurrentSpread}, IsAiming: {playerAim.isAiming}, IsMoving: {playerMovement.IsMoving}, IsCrouching: {playerMovement.IsCrouching}");
        float targetSpread;

        if (!playerAim.isAiming)
        {
            // Hofteskyting
            targetSpread = hipFireSpread;
            aimTimer = 0f;

            if (playerMovement.IsMoving)
                targetSpread += movingSpreadAddition;
        }
        else
        {
            // Sikter - bygger presisjon over tid, men kun mens man står stille
            if (!playerMovement.IsMoving)
                aimTimer += Time.deltaTime;
            else
                aimTimer = 0f; // beveger seg mens man sikter -> mister opparbeidet presisjon

            float t = Mathf.Clamp01(aimTimer / aimInTime);
            targetSpread = Mathf.Lerp(aimSpreadMax, aimSpreadMin, t);

            if (playerMovement.IsMoving)
                targetSpread += movingSpreadAddition * 0.5f; // litt mindre straff enn hofteskyting siden man tross alt sikter

            if (playerMovement.IsCrouching)
                targetSpread *= crouchSpreadMultiplier;
        }

        CurrentSpread = Mathf.MoveTowards(CurrentSpread, targetSpread, spreadChangeSpeed * Time.deltaTime);
    }
}