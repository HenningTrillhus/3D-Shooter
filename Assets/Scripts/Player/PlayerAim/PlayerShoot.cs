using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzlePoint; // tomt GameObject plassert ved løpet på pistolen
    public float fireRate = 0.8f;

    private float nextFireTime = 0f;

    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;
    private AimSpread aimSpread; 

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        aimSpread = GetComponent<AimSpread>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        float randomAngle = Random.Range(-aimSpread.CurrentSpread / 2f, aimSpread.CurrentSpread / 2f);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Quaternion bulletRotation = spreadRotation * muzzlePoint.rotation;

        Instantiate(bulletPrefab, muzzlePoint.position, bulletRotation);

        animator.SetTrigger("Shoot");

        if (muzzleFlash != null)
            StartCoroutine(FlashMuzzle());

        if (CameraFollow.Instance != null)
            CameraFollow.Instance.Shake(0.15f, 0.3f);
    }

    IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        muzzleFlash.SetActive(false);
    }
}