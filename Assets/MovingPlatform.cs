using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Vector3 target;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Moves via script, not forces
        target = pointB.position;
    }

    private void FixedUpdate() // Physics moves belong in FixedUpdate
    {
        Vector3 newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, target) < 0.05f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
}