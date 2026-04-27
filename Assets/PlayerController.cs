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
    private float verticalVelocity;
    private Transform cameraTransform;

    //Jump variables
    private bool jumpQueued = false;
    public int maxJumps = 2;
    private int currentJumps;
    private bool wasGrounded;

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
        //Jumping & grounding logic
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            currentJumps = 0;
        }
        if (!isGrounded && wasGrounded && verticalVelocity <= 0)
        {
            currentJumps = 1;
        }
        if (jumpQueued == true && currentJumps < maxJumps)
        {
            currentJumps++;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
            animator.SetTrigger("Jump");
        }
        jumpQueued = false;
        wasGrounded = isGrounded; //Der vil være en halv frame hvor wasGrounded og isGrounded er det modsatte af hinanden - på det tidspunkt aktiveres if statement 2.

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
        controller.Move(velocity * Time.deltaTime);

        //Rotation logic
        if (MoveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        //Animation logic
        animator.SetFloat("Speed", MoveDirection.magnitude);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);

    }
}
