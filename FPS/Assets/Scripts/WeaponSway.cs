using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Weapon sway")]
    public float swayIntensity;
    public float swaySmoothing;

    [Header("Weapon tilt")]
    public float tiltIntensity;
    public float maxTiltAmount;
    public float tiltSmoothing;

    [Space]
    public bool tiltX = true;
    public bool tiltY = true;
    public bool tiltZ = true;

    private Quaternion originRotation;

    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        originRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxis("Mouse Y");

        UpdateSway();
        UpdateTilt();
    }

    private void UpdateSway()
    {
        Quaternion xAdjustment = Quaternion.AngleAxis(-swayIntensity * horizontalInput, Vector3.up);
        Quaternion yAdjustment = Quaternion.AngleAxis(swayIntensity * verticalInput, Vector3.right);
        Quaternion targetRotation = originRotation * xAdjustment * yAdjustment;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothing);
    }

    private void UpdateTilt()
    {
        float xAdjustment = Mathf.Clamp(verticalInput * tiltIntensity, -maxTiltAmount, maxTiltAmount);
        float yAdjustment = Mathf.Clamp(horizontalInput * tiltIntensity, -maxTiltAmount, maxTiltAmount);

        Quaternion targetRotation = Quaternion.Euler(new Vector3(tiltX ? -xAdjustment : 0f, tiltY ? yAdjustment : 0f, tiltZ ? yAdjustment : 0f));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation * originRotation, Time.deltaTime * tiltSmoothing);
    }
}
