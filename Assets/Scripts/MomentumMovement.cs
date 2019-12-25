using UnityEngine;

[RequireComponent(typeof(CharacterController))]
class MomentumMovement : MonoBehaviour
{
    public GameObject playerCamera;

    CharacterController controller;
    float speed = 400f;
    Vector3 lastVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastVelocity = controller.velocity;
    }

    Vector3 ScaleDirectionVector(Vector3 direction)
    {
        float multiplier = 1 / (Mathf.Abs(direction.x) + Mathf.Abs(direction.z));
        return new Vector3(
            direction.x * multiplier,
            0,
            direction.z * multiplier
        );
    }

    void Move()
    {
        Vector3 moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
        moveVector *= speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
    }

    void RotateToVelocity()
    {
        Vector3 lookAt = transform.position + controller.velocity.normalized;
        Vector3 targetPostition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        if (targetPostition - transform.position != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(targetPostition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 500 * Time.deltaTime);
        }

    }

    void TiltToAcceleration()
    {
        // WHENEVER HE ACCELERATES it tilts in that direction!
        // When he collides face first into the wall, it tilts backwards

        Vector3 centerOfMass = controller.center + controller.transform.position;
        Vector3 acceleration = controller.velocity / Time.deltaTime - lastVelocity;
        Vector3 tilt = new Vector3(acceleration.z, 0, acceleration.x) / 3; // Rotation flipped in some directions        
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + tilt);
        controller.transform.GetChild(0).rotation = Quaternion.Lerp(controller.transform.GetChild(0).rotation, targetRotation, 10 * Time.deltaTime);
    }

    void Update()
    {
        Move();
        RotateToVelocity();
        TiltToAcceleration();
        lastVelocity = controller.velocity / Time.deltaTime;
    }
}