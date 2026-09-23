using UnityEngine;

public class AimReticle : MonoBehaviour
{
    private PlayerAim playerAim;
    public GameObject reticleObject; // prefab/objekt for prikken i scenen

    void Start()
    {
        playerAim = GetComponent<PlayerAim>();

        if (reticleObject != null)
            reticleObject.SetActive(false);
    }

    void Update()
    {
        if (reticleObject == null) return;

        bool show = playerAim.isAiming;
        reticleObject.SetActive(show);

        if (show)
        {
            Vector3 pos = playerAim.AimPoint;
            pos.y += 0.05f; // løft litt over bakken for å unngå z-fighting
            reticleObject.transform.position = pos;
        }
    }
}