using UnityEngine;

public class AimConeVisual : MonoBehaviour
{
    private AimSpread aimSpread;
    private PlayerAim playerAim;

    public Transform leftPivot;
    public Transform rightPivot;

    [Header("Visuell skalering (halv-vinkel per side)")]
    public float visualMinAngle = 1f;   // gir 2° totalt mellom linjene
    public float visualMaxAngle = 30f;  // gir 60° totalt mellom linjene

    [Header("Spread-verdier dette skal mappes fra")]
    public float spreadAtVisualMin = 2f;  // tilsvarer AimSpread sin aimSpreadMin
    public float spreadAtVisualMax = 20f; // tilsvarer verste tilfelle (hipfire + moving)

    private float leftBaseX, leftBaseZ;
    private float rightBaseX, rightBaseZ;

    void Awake()
    {
        aimSpread = GetComponent<AimSpread>();
        playerAim = GetComponent<PlayerAim>();
    }

    void Start()
    {
        leftBaseX = leftPivot.localEulerAngles.x;
        leftBaseZ = leftPivot.localEulerAngles.z;

        rightBaseX = rightPivot.localEulerAngles.x;
        rightBaseZ = rightPivot.localEulerAngles.z;
    }

    void Update()
    {
        bool show = playerAim.isAiming;
        leftPivot.gameObject.SetActive(show);
        rightPivot.gameObject.SetActive(show);

        if (!show) return;

        float t = Mathf.InverseLerp(spreadAtVisualMin, spreadAtVisualMax, aimSpread.CurrentSpread);
        float angle = Mathf.Lerp(visualMinAngle, visualMaxAngle, t);

        //Debug.Log($"CurrentSpread: {aimSpread.CurrentSpread}, t: {t}, angle: {angle}");

        leftPivot.localEulerAngles = new Vector3(leftBaseX, -angle, leftBaseZ);
        rightPivot.localEulerAngles = new Vector3(rightBaseX, angle, rightBaseZ);
    }
}