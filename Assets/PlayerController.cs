using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float Gravity = -20f;
    public Animator animator;

    private CharacterController controller;
    private Vector2 moveInput;
    private bool jumpQueued;
    private float verticalVelocity;
    private Transform cameraTransform;
    private bool isGrounded;
    public int maxJumps = 2;
    private int jumpCount;

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
         moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpQueued = true;
        }

    }
    // Update is called once per frame
    void Update()
    {
        CheckForPlatform();
        JumpLogic();
        Movement();
    }



    private void JumpLogic()
    {
        isGrounded = controller.isGrounded;
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            jumpCount = 0;
        }
        if (jumpQueued && jumpCount < maxJumps)
        {
            jumpCount++;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
            animator.SetTrigger("Jump");
        }
        jumpQueued = false;

    }
    private void Movement()
    {
        //Movement logic
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 MoveDirection = moveInput.x * right + moveInput.y * forward;
        Debug.Log($"MoveDirection: {MoveDirection.magnitude}");

        if (MoveDirection.magnitude > 1f)
        {
            MoveDirection.Normalize();
        }

        Vector3 velocity = MoveDirection * moveSpeed;
        verticalVelocity += Gravity * Time.deltaTime;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime + PlatformMovement());

        //Rotation logic
        if (MoveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        animator.SetFloat("Speed", MoveDirection.magnitude);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
    private void CheckForPlatform()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            if(hit.collider.TryGetComponent<MovingPlatform>(out var platform))
            {
                currentPlatform = platform.transform;
                
                if (lastPlatformPosition == Vector3.zero)
                {
                    lastPlatformPosition = currentPlatform.position;
                }   
            }
        }
        else
        {
            currentPlatform = null;
        }       
    }
    private Vector3 PlatformMovement()
    {
        if (currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
            lastPlatformPosition = currentPlatform.position;
            return platformMovement;
        }
        else
        {
            lastPlatformPosition = Vector3.zero;
            return Vector3.zero;
        }
    }
}
