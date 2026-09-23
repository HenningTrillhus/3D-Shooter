using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private PlayerAim playerAim;
    private PlayerMovement playerMovement;
    public float rotateSpeed = 10f;

    void Start()
    {
        playerAim = GetComponent<PlayerAim>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!playerAim.isAiming) return;
        if (playerMovement.IsMoving) return; // beveger seg -> PlayerMovement styrer rotasjonen i stedet

        Quaternion targetRotation = Quaternion.LookRotation(playerAim.AimDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}