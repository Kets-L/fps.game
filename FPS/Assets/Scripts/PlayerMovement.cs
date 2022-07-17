using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public CapsuleCollider playerCollider;
    public Transform cameraPos;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    private float moveSpeed;
    private bool sprinting;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMovementMultiplier;
    public float grativyDownMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float crouchingSpeed;
    public float cameraSmoothAmount;
    public float cameraCrouchHeight;
    private float startYScale;
    private float startCameraHeight;
    private float cameraHeightVelocity;
    private bool crouching;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    private bool sliding;

    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Space]

    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode slideKey = KeyCode.LeftControl; 

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    bool isGrounded;

    [Header("Slope Handeling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    float horizontalInput;
    float verticalInput;
    float forwardVelocity;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = playerCollider.height;
        startCameraHeight = cameraPos.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        SpeedControl();
        StateHandler();
        CheckForWall();

        // Groundcheck
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        forwardVelocity = orientation.InverseTransformDirection(rb.velocity).z;
    }

    private void FixedUpdate()
    {
        MovePlayer();

        float desiredPlayerHeight = (crouching || sliding) ? crouchYScale : startYScale;
        float desiredCameraHeight = (crouching || sliding) ? cameraCrouchHeight : startCameraHeight;

        if ((playerCollider.height != desiredPlayerHeight || cameraPos.localPosition.y != desiredCameraHeight) && isGrounded)
        {
            AdjustHeight(desiredPlayerHeight, desiredCameraHeight);
        }

        if (sliding)
            SlidingMovement();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(sprintKey))
            sprinting = true;

        if (!Input.GetKey(sprintKey) && forwardVelocity < 8)
        {
            sprinting = false;
        }

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch
        //crouching = Input.GetKey(crouchKey) && forwardVelocity < 8 && !sliding;
        if (Input.GetKeyDown(crouchKey) && forwardVelocity < 8 && !sliding) {
            crouching = true;
        }
        else if (!Input.GetKey(crouchKey) && !Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f, groundLayer))
        {
            crouching = false;
        }

        // Slide
        if (Input.GetKeyDown(slideKey) && forwardVelocity >= 8 && isGrounded)
        {
            StartSlide();
        }

        //if (Input.GetKeyUp(slideKey) && sliding)
        //{
        //    StopSlide();
        //}
    }

    private void StateHandler()
    {
        // Sprinting
        if (isGrounded && sprinting)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Crouching
        if (Input.GetKey(crouchKey) && isGrounded && !sliding) 
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        rb.useGravity = !OnSlope();

        // Move speed
        if (isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMovementMultiplier, ForceMode.Force);

        // Velocity in the air
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * grativyDownMultiplier * Time.deltaTime;
        }
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
        Vector3 flatVelY = new Vector3(0f, rb.velocity.y, 0f);

        // limit velocity
        if (flatVelY.magnitude > 20)
        {
            Vector3 limitedVelY = flatVelY.normalized * 10;
            rb.velocity = new Vector3(rb.velocity.x, limitedVelY.y, rb.velocity.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void AdjustHeight(float desiredPlayerHeight, float desiredCameraHeight)
    {
        float center = desiredPlayerHeight / 2 - 1;

        playerCollider.height = Mathf.Lerp(playerCollider.height, desiredPlayerHeight, crouchingSpeed);
        playerCollider.center = Vector3.Lerp(playerCollider.center, new Vector3(0, center, 0), crouchingSpeed);

        float cameraHeight = Mathf.SmoothDamp(cameraPos.localPosition.y, desiredCameraHeight, ref cameraHeightVelocity, cameraSmoothAmount);
        cameraPos.localPosition = new Vector3(cameraPos.localPosition.x, cameraHeight, cameraPos.localPosition.z);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void StartSlide()
    {
        sliding = true;

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        // Sliding normal 
        if (!OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(orientation.forward.normalized * slideTimer * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // Sliding down a slope
        else
        {
            rb.AddForce(GetSlopeMoveDirection(orientation.forward) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        sliding = false;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StartWallRun()
    {

    }

    private void WallRunningMovement()
    {

    }

    private void StopWallRun()
    {

    }
}
