using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    public bool IsReloading => isReloading;

    public GameObject bulletPrefab;
    public Transform muzzlePoint;
    public float fireRate = 0.8f;

    private float nextFireTime = 0f;

    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Magasin")]
    public int magazineSize = 9;
    public float reloadTime = 1.5f;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("UI")]
    public TextMeshProUGUI ammoText; // dra inn AmmoText her

    private Animator animator;
    private AimSpread aimSpread;

    void Start()
    {
        animator = GetComponent<Animator>();
        aimSpread = GetComponent<AimSpread>();
        currentAmmo = magazineSize;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        float randomAngle = Random.Range(-aimSpread.CurrentSpread / 2f, aimSpread.CurrentSpread / 2f);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

        // Bruk transform.forward (kroppens retning) som basis - samme som de visuelle stripene
        Quaternion baseRotation = Quaternion.LookRotation(transform.forward);
        Quaternion bulletRotation = spreadRotation * baseRotation;

        Instantiate(bulletPrefab, muzzlePoint.position, bulletRotation);

        animator.SetTrigger("Shoot");

        if (muzzleFlash != null)
        {
            StopCoroutine(nameof(FlashMuzzle));
            StartCoroutine(nameof(FlashMuzzle));
        }

        if (CameraFollow.Instance != null)
            CameraFollow.Instance.Shake(0.15f, 0.3f);

        aimSpread.AddRecoil();
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reloding");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        UpdateAmmoUI();
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {magazineSize}";
    }

    IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        muzzleFlash.SetActive(false);
    }
}