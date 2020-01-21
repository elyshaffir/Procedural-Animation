using UnityEngine;

class MomentumMovement : MonoBehaviour
{
    const float MovementSpeed = 1000f;
    const float MaxGroundAngle = 50f;
    const float MaxVelocity = 50f;
    const float SlowDownRate = 1.06f;
    const float RotationSpeed = 5f;
    const float TiltAngle = 20f;

    public GameObject playerCamera;
    public GameObject playerModel;
    public LayerMask ground;

    Rigidbody rb;
    bool grounded;
    Vector3 moveVector;
    float forceAngle;
    Vector3 extraForce;

    public bool isGrounded()
    {
        return grounded;
    }

    public float getSpeed()
    {
        return rb.velocity.magnitude;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CalculateMoveVector();
        CalculateGrounded();
        Move();
        RotateToVelocity();
        TiltToAcceleration();
    }

    void CalculateMoveVector()
    {
        moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
    }

    Vector3 ScaleDirectionVector(Vector3 direction)
    {
        return new Vector3(
            direction.x,
            0,
            direction.z
        ).normalized * MovementSpeed * Time.deltaTime;
    }

    void CalculateGrounded()
    {
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position, -Vector3.up, out hit, 2, ground);
        forceAngle = Vector3.Angle(hit.normal, Vector3.up);
        if (grounded)
        {
            Vector3 extraForceDirection = Vector3.Cross(hit.normal, Vector3.Cross(moveVector, Vector3.up));
            extraForce = extraForceDirection.normalized * MovementSpeed * Time.deltaTime;
        }
    }
    void Move()
    {
        if (grounded && forceAngle < MaxGroundAngle)
        {
            rb.AddForce(extraForce);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
        }
        if (moveVector == Vector3.zero)
        {
            rb.velocity /= SlowDownRate;
        }
    }

    void RotateToVelocity()
    {
        Vector3 lookAt = transform.position + rb.velocity.normalized;
        Vector3 targetPosition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        if (targetPosition - transform.position != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);
        }

    }

    void TiltToAcceleration()
    {
        Vector3 tilt = CalculateTilt(moveVector);
        Quaternion targetRotation = Quaternion.Euler(tilt);
        playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    Vector3 CalculateTilt(Vector3 acceleration)
    {
        Vector3 tiltAxis = Vector3.Cross(acceleration, Vector3.up);
        float angle = Mathf.Clamp(-acceleration.magnitude, -TiltAngle, TiltAngle);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
        return targetRotation.eulerAngles;
    }
}