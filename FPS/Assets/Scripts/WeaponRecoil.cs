using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("References")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    public Vector3 startPostion = new Vector3(0.25f, -0.08f, 0.25f);

    [Header("Speed settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;

    [Space(10)]

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;

    [Header("Amount settings")]
    public Vector3 recoilRotationHipfire = new Vector3(10, 5, 7);
    public Vector3 recoilKickBackHipfire = new Vector3(0.015f, 0f, -0.2f);

    [Space(10)]

    public Vector3 recoilRotationAiming = new Vector3(10, 4, 6);
    public Vector3 recoilKickBackAiming = new Vector3(0.015f, 0f, -0.2f);

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 rotation;

    [HideInInspector]
    public bool aiming;

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, startPostion, positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        rotation = Vector3.Slerp(rotation, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rotation);
    }

    public void AddRecoil()
    {
        if (aiming)
        {
            rotationalRecoil += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickBackAiming.x, recoilKickBackAiming.x), recoilKickBackAiming.y, recoilKickBackAiming.z);
        } else
        {
            rotationalRecoil += new Vector3(-recoilRotationHipfire.x, Random.Range(-recoilRotationHipfire.y, recoilRotationHipfire.y), Random.Range(-recoilRotationHipfire.z, recoilRotationHipfire.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickBackHipfire.x, recoilKickBackHipfire.x), Random.Range(-recoilKickBackHipfire.y, recoilKickBackHipfire.y), recoilKickBackHipfire.z);
        }
    }
}
