using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    public Transform player;
    private PlayerAim playerAim;
    private Camera cam;

    public Vector3 offset = new Vector3(6f, 10f, -2.35f);
    public float distance = 8f;

    [Header("Aim blend (skjermbasert)")]
    [Range(0f, 1f)]
    public float aimBlendAmount = 0.5f;
    public float maxAimOffsetSide = 6f;
    public float maxAimOffsetUp = 4f;
    public float maxAimOffsetDown = 2f;
    public float blendSpeed = 5f;

    private float currentBlend = 0f;

    [Header("Shake")]
    private float shakeMagnitude = 0f;
    private float shakeTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        if (player != null)
            playerAim = player.GetComponent<PlayerAim>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        bool isAiming = playerAim != null && playerAim.isAiming;
        float targetBlend = isAiming ? 1f : 0f;
        currentBlend = Mathf.MoveTowards(currentBlend, targetBlend, blendSpeed * Time.deltaTime);

        Vector3 focusPoint = player.position;

        if (currentBlend > 0f)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 viewportPos = cam.ScreenToViewportPoint(mouseScreenPos);

            // -1 til 1, klemt slik at vi ikke trenger å dra musa utenfor skjermen
            float nx = -Mathf.Clamp(viewportPos.x * 2f - 1f, -1f, 1f); // snudd
            float ny = -Mathf.Clamp(viewportPos.y * 2f - 1f, -1f, 1f); // snudd

            Vector3 camFlatForward = new Vector3(offset.x, 0f, offset.z).normalized;
            Vector3 camFlatRight = Vector3.Cross(Vector3.up, camFlatForward).normalized;

            float sideAmount = nx * maxAimOffsetSide * aimBlendAmount * 2f;
            float forwardAmount = ny >= 0f
                ? ny * maxAimOffsetUp * aimBlendAmount * 2f
                : ny * maxAimOffsetDown * aimBlendAmount * 2f;

            Vector3 clampedOffset = camFlatRight * sideAmount + camFlatForward * forwardAmount;

            Vector3 aimFocusPoint = player.position + clampedOffset;
            focusPoint = Vector3.Lerp(player.position, aimFocusPoint, currentBlend);
        }

        Vector3 shakeOffset = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shakeOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = focusPoint + offset.normalized * distance + shakeOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}