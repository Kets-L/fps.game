using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    [Header("Gun stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    [Header("Aim down sight")]
    public Vector3 adsPosition;
    public Quaternion adsRotation;
    public float adsSpeed;
    private Vector3 hipfirePosition;
    private Quaternion hipfireRotation;

    [Header("Graphics")]
    public GameObject bulletHoleGraphic;
    public ParticleSystem muzzleFlash;

    bool shooting, readyToShoot, reloading;

    // Start is called before the first frame update
    void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        hipfirePosition = transform.localPosition;
        hipfireRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
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

        AimDownSight(Input.GetKey(KeyCode.Mouse1));
    }

    private void Shoot()
    {
        readyToShoot = false;

        Debug.Log(bulletsLeft);

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {

        }

        // Graphics
        muzzleFlash.Play();
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));

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
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public void AimDownSight(bool aiming)
    {
        if (aiming && !reloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, adsPosition, Time.deltaTime * adsSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, adsRotation, Time.deltaTime * adsSpeed);
        } 
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipfirePosition, Time.deltaTime * adsSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, hipfireRotation, Time.deltaTime * adsSpeed);
        }
    }
}
