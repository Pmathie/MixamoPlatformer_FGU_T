using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public Animator animator;

    private CharacterController controller;
    private Vector2 moveInput;
    private bool jumpQueued;
    private float verticalVelocity;
    private Transform cameraTransform;
    private bool isGrounded;
    public int maxJumps = 2;
    private int jumpCount;

    private MovingPlatform currentPlatform;
  

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
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
        Vector3 moveDirection = moveInput.x * right + moveInput.y * forward;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 velocity = moveDirection * moveSpeed;
        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;
        if (currentPlatform != null)
        {
            controller.Move(currentPlatform.DeltaPosition);
        }
        
        controller.Move(velocity * Time.deltaTime);

        //Rotation logic
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
    private void CheckForPlatform()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            if(hit.collider.TryGetComponent<MovingPlatform>(out var platform))
            {
                currentPlatform = platform;
                
  
                return;
            }
        }

        currentPlatform = null;
    }
    
}
