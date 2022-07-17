using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;
    public Animator animator;

    [Header("Gun stats")]
    public int damage;
    public float timeBetweenShooting, spread, spreadHipfireMultiplier, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    [Header("Aim down sight")]
    public Transform adsPositionTransform;
    public Vector3 adsPosition;
    public Quaternion adsRotation;
    public float adsSpeed;
    private Vector3 hipfirePosition;
    private Quaternion hipfireRotation;

    [Header("Graphics")]
    public GameObject bulletHoleGraphic;
    public GameObject bulletHoleEnemyGraphic;
    public ParticleSystem muzzleFlash;
    public CameraRecoil cameraRecoil;
    public WeaponRecoil weaponRecoil;
    public TextMeshProUGUI bulletAmountText;

    private bool shooting, readyToShoot, reloading, aiming;

    // Start is called before the first frame update
    void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        hipfirePosition = adsPositionTransform.localPosition;
        hipfireRotation = adsPositionTransform.localRotation;

        animator.keepAnimatorControllerStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        UpdateAnimator();

        bulletAmountText.SetText(bulletsLeft + " | " + magazineSize);
    }

    private void PlayerInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        aiming = Input.GetKey(KeyCode.Mouse1);
        AimDownSight();
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Spread
        float appliedSpread = 0f;
        if (!aiming)
        {
            appliedSpread = spread;
            appliedSpread *= spreadHipfireMultiplier;
        }

        float x = Random.Range(-appliedSpread, appliedSpread);
        float y = Random.Range(-appliedSpread, appliedSpread);
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, x);

        // Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {
            if (rayHit.collider.CompareTag("Enemy")) 
            {
                Instantiate(bulletHoleEnemyGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
            else
            {
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }

        // Graphics
        muzzleFlash.Play();

        cameraRecoil.aiming = aiming;
        cameraRecoil.AddRecoil();

        weaponRecoil.aiming = aiming;
        weaponRecoil.AddRecoil();

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        animator.SetTrigger("Reloading");
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public void AimDownSight()
    {
        if (aiming && !reloading)
        {
            adsPositionTransform.localPosition = Vector3.Lerp(adsPositionTransform.localPosition, adsPosition, Time.deltaTime * adsSpeed);
            adsPositionTransform.localRotation = Quaternion.Slerp(adsPositionTransform.localRotation, adsRotation, Time.deltaTime * adsSpeed);
        } 
        else
        {
            adsPositionTransform.localPosition = Vector3.Lerp(adsPositionTransform.localPosition, hipfirePosition, Time.deltaTime * adsSpeed);
            adsPositionTransform.localRotation = Quaternion.Slerp(adsPositionTransform.localRotation, hipfireRotation, Time.deltaTime * adsSpeed);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("Aiming", aiming);
    }
}
