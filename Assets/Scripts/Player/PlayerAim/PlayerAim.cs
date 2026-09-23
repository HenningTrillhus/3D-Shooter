using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Animator animator;
    private Camera mainCamera;
    private PlayerMovement playerMovement;

    public int layerIndex = 1;
    private string layerName = "AimingLayer";
    public LayerMask groundLayer;

    [Header("Hastighet")]
    public float increaseSpeed = 8f;
    public float decreaseSpeed = 4f;

    [Header("Vinkelbegrensning mens man beveger seg")]
    public float maxAngleWhileMoving = 45f; // maks avvik fra kroppens retning for å løfte våpen mens man går

    private float currentWeight = 0f;

    public bool isAiming => Mouse.current.rightButton.isPressed;
    public Vector3 AimDirection { get; private set; }
    public Vector3 AimPoint { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        layerIndex = animator.GetLayerIndex(layerName);
        AimDirection = transform.forward;
    }

    void Update()
    {
        bool shouldLift = isAiming;

        // Mens man beveger seg, krev at siktevinkelen stemmer noenlunde med kroppens retning
        if (isAiming && playerMovement != null && playerMovement.IsMoving)
        {
            float angle = Vector3.Angle(transform.forward, AimDirection);
            if (angle > maxAngleWhileMoving)
                shouldLift = false;
        }

        float targetWeight = shouldLift ? 1f : 0f;
        float speed = shouldLift ? increaseSpeed : decreaseSpeed;

        currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, speed * Time.deltaTime);
        animator.SetLayerWeight(layerIndex, currentWeight);

        if (isAiming)
        {
            UpdateAimDirection();
        }
    }

    void UpdateAimDirection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPoint = hit.point;
            AimPoint = targetPoint;

            targetPoint.y = transform.position.y;

            Vector3 dir = (targetPoint - transform.position);
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
                AimDirection = dir.normalized;
        }
    }
}