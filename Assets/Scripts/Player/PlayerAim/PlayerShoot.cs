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
    private PlayerAim playerAim;

    void Start()
    {
        animator = GetComponent<Animator>();
        aimSpread = GetComponent<AimSpread>();
        playerAim = GetComponent<PlayerAim>();
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

        if (Mouse.current.leftButton.wasPressedThisFrame
            && playerAim.isAiming
            && Time.time >= nextFireTime
            && currentAmmo > 0)
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

        NotifyNearbyEnemies(); // nytt
    }

    void NotifyNearbyEnemies()
    {
        float noiseRadius = 100f;
        Collider[] hits = Physics.OverlapSphere(transform.position, noiseRadius);

        Debug.Log("Fant " + hits.Length + " colliders innenfor radius");

        foreach (Collider hit in hits)
        {
            Debug.Log("Sjekker: " + hit.name);

            EnemyController controller = hit.GetComponentInParent<EnemyController>();
            if (controller != null)
            {
                Debug.Log("Fant EnemyController på: " + controller.name);
                controller.EnterUnderAttack();
            }
        }
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