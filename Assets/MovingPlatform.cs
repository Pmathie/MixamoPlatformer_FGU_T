using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Transform target;
    private Vector3 lastPosition;
    public Vector3 DeltaPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = pointB;
        transform.position = pointA.position;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Physics.SyncTransforms();
        
        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
        
        if(Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (target == pointB)
            {
                target = pointA;
            }
            else
            {
                target = pointB;
            }

        }
    }
}