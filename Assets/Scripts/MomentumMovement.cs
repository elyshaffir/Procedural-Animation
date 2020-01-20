using UnityEngine;

class MomentumMovement : MonoBehaviour
{
    const float MovementSpeed = 1000f;
    const float MaxVelocity = 50;
    const float RotationSpeed = 5;
    const float TiltAngle = 20;

    public GameObject playerCamera;
    public GameObject playerModel;
    public LayerMask ground;

    Rigidbody rb;
    Vector3 lastVelocity;
    bool grounded;
    Vector3 moveVector;
    Vector3 extraForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastVelocity = rb.velocity;
    }

    void FixedUpdate()
    {
        CalculateMoveVector();
        CalculateGrounded();
        Move();
        RotateToVelocity();
        TiltToAcceleration();
        lastVelocity = rb.velocity;
    }

    void CalculateMoveVector()
    {
        moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
    }

    void CalculateGrounded()
    {
        RaycastHit hit;
        Vector3 groundedRaycastPosition = transform.position;
        grounded = Physics.Raycast(groundedRaycastPosition, -Vector3.up, out hit, 2, ground);
        if (grounded)
        {
            Vector3 extraForceDirection = Vector3.Cross(hit.normal, Vector3.Cross(moveVector, Vector3.up));
            extraForce = extraForceDirection.normalized * MovementSpeed * Time.deltaTime;
        }
    }

    void Move()
    {
        if (grounded)
        {
            rb.AddForce(moveVector * 0 + extraForce);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
        }
        if (moveVector == Vector3.zero)
        {
            rb.velocity /= 1.03f;
        }
    }

    Vector3 ScaleDirectionVector(Vector3 direction)
    {
        return new Vector3(
            direction.x,
            0,
            direction.z
        ).normalized * MovementSpeed * Time.deltaTime;
    }

    void RotateToVelocity()
    {
        Vector3 lookAt = transform.position + rb.velocity.normalized;
        Vector3 targetPosition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        if (targetPosition - transform.position != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, RotationSpeed * Time.deltaTime);
        }

    }

    void TiltToAcceleration()
    {
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.deltaTime;
        Debug.Log(acceleration.magnitude);
        Debug.DrawLine(transform.position, transform.position + acceleration, Color.blue); // acceleration changes drastically instead of losing magnitude first
        Vector3 tilt = CalculateTilt(acceleration);
        Quaternion targetRotation = Quaternion.Euler(tilt);
        playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    Vector3 CalculateTilt(Vector3 acceleration)
    {
        acceleration.y = 0;
        Vector3 tiltAxis = Vector3.Cross(acceleration, Vector3.up);
        float angle = Mathf.Clamp(-acceleration.magnitude, -TiltAngle, TiltAngle);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
        return targetRotation.eulerAngles;
    }
}