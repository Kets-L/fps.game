using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSystem : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator.keepAnimatorControllerStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Knifing", Input.GetKey(KeyCode.Mouse0));
    }
}
