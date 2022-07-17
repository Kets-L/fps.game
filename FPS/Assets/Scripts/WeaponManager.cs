using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform primaryWeaponHolder;
    public Transform secondaryWeaponHolder;
    public Transform knifeHolder;

    private List<Transform> weapons = new List<Transform>();

    private bool switchingWeapon;

    public WeaponState currentWeaponState;
    private WeaponState previousWeaponState;
    public enum WeaponState
    {
        primary,
        secondary,
        knife
    }

    // Start is called before the first frame update
    void Start()
    {
        weapons.Add(primaryWeaponHolder);
        weapons.Add(secondaryWeaponHolder);
        weapons.Add(knifeHolder);

        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponState = WeaponState.primary;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponState = WeaponState.secondary;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeaponState = WeaponState.knife;
        }

        if (currentWeaponState != previousWeaponState && !switchingWeapon)
        {
            SelectWeapon(); 
        }
    }

    private void SelectWeapon()
    {
        //switchingWeapon = true;

        //GetPreviousWeapon().SetActive(true);
        //Animator previousWeaponAnimator;

        //if (previousWeaponState == WeaponState.knife)
        //{
        //    var weapon = GetPreviousWeapon().GetComponent<KnifeSystem>();
        //    previousWeaponAnimator = weapon.animator;
        //}
        //else
        //{
        //    var weapon = GetPreviousWeapon().GetComponent<GunSystem>();
        //    previousWeaponAnimator = weapon.animator;
        //}

        //previousWeaponAnimator.SetTrigger("PutAway");

        //while (previousWeaponAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        //{
        //    yield return null;
        //}

        for (int i = 0; i < weapons.Count; i++)
        {
            if (i == (int) currentWeaponState)
            {
                weapons[i].GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                weapons[i].GetChild(0).gameObject.SetActive(false);
            }
        }

        //Animator currentWeaponAnimator;
        //if (currentWeaponState == WeaponState.knife)
        //{
        //    currentWeaponAnimator = GetCurrentWeapon().GetComponent<KnifeSystem>().animator;
        //}
        //else
        //{
        //    currentWeaponAnimator = GetCurrentWeapon().GetComponent<GunSystem>().animator;
        //}
        //currentWeaponAnimator.SetTrigger("GetUp");

        previousWeaponState = currentWeaponState;
        //switchingWeapon = false;
    }

    public GameObject GetCurrentWeapon()
    {
        switch (currentWeaponState)
        {
            case WeaponState.primary:
                return GetPrimaryWeapon();
            case WeaponState.secondary:
                return GetSecondaryWeapon();
            case WeaponState.knife:
                return GetKnife();
            default:
                return null;
        }
    }

    public GameObject GetPreviousWeapon()
    {
        switch (previousWeaponState)
        {
            case WeaponState.primary:
                return GetPrimaryWeapon();
            case WeaponState.secondary:
                return GetSecondaryWeapon();
            case WeaponState.knife:
                return GetKnife();
            default:
                return null;
        }
    }

    private GameObject GetPrimaryWeapon()
    {
        return primaryWeaponHolder.GetChild(0).gameObject;
    }

    private GameObject GetSecondaryWeapon()
    {
        return secondaryWeaponHolder.GetChild(0).gameObject;
    }

    private GameObject GetKnife()
    {
        return knifeHolder.GetChild(0).gameObject;
    }
}
