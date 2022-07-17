using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUIUpdater : MonoBehaviour
{
    public GameObject crosshair;

    // Update is called once per frame
    void Update()
    {
        UpdateCrosshair();
    }

    void UpdateCrosshair()
    {
        crosshair.SetActive(!Input.GetKey(KeyCode.Mouse1));
    }
}
