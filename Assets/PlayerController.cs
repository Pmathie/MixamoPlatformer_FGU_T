using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    private CharacterController controller;

    public Transform cameraTransform;
    private Vector2 moveInput;
    private bool jumpQueued;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)

    {
        
       // if (context.performed)
        {
            jumpQueued = true;
            
        }
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        Debug.Log("Is Grounded: " + isGrounded);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (jumpQueued && isGrounded)
        {
            Debug.Log("Executing jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
        jumpQueued = false;

        //KAMERA-BASERET MOVEMENT
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;


        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        //Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y); IKKE KAMERA-BASERET

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        //Prøv uden denne kode først
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        //STOP

        animator.SetFloat("Speed", moveInput.magnitude);
        animator.SetBool("Grounded", controller.isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
}