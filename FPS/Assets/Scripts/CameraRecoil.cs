using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil stats")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 25f;

    [Header("Hipfire")]
    public Vector3 recoilRotationHipfire = new Vector3(2f, 2f, 2f);

    [Header("Aiming")]
    public Vector3 recoilRotationAiming = new Vector3(0.5f, 0.5f, 0.5f);

    private Vector3 currentRotation;
    private Vector3 rotation;

    [HideInInspector]
    public bool aiming;

    void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        rotation = Vector3.Slerp(rotation, currentRotation, rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(rotation);
    }

    public void AddRecoil()
    {
        if (aiming)
        {
            currentRotation += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
        } 
        else
        {
            currentRotation += new Vector3(-recoilRotationHipfire.x, Random.Range(-recoilRotationHipfire.y, recoilRotationHipfire.y), Random.Range(-recoilRotationHipfire.z, recoilRotationHipfire.z));
        }
    }
}
