using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAim playerAim;

    public float playerSpeed = 5f;
    public float sprintSpeed = 8f;
    public float turnSpeed = 10f;
    public bool IsMoving { get; private set; }
    public bool IsCrouching => isCrouching;
    public bool IsSprinting => isSprinting;

    private bool isSprinting;   // <-- lagt til
    private bool isCrouching;   // <-- lagt til

    private Animator Animator;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        playerAim = GetComponent<PlayerAim>();
    }

    private void ctrlPressed()
    {
        if (!isSprinting && !isCrouching)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;

        IsMoving = input.sqrMagnitude > 0.01f;
        isSprinting = IsMoving && !isCrouching && Keyboard.current.leftShiftKey.isPressed && !playerAim.isAiming;

        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            ctrlPressed();
        }

        if (Animator != null)
        {
            Animator.SetBool("IsMoving", IsMoving);
            Animator.SetBool("IsSprinting", isSprinting);
            Animator.SetBool("IsCrouching", isCrouching);
        }

        if (!IsMoving) return;

        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight = cam.right; camRight.y = 0; camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        float currentSpeed = isSprinting ? sprintSpeed : (!isCrouching ? playerSpeed : playerSpeed * 0.5f);
        transform.position += moveDir * currentSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}