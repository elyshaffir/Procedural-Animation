using UnityEngine;

[RequireComponent(typeof(CharacterController))]
class MomentumMovement : MonoBehaviour
{

    /*
        Can chage direction very easily, not very realistic.
    */

    const float MovementSpeed = 500f;
    const float RotationSpeed = 5;
    const float TiltAngle = 20;

    public GameObject playerCamera;
    public GameObject playerModel;

    CharacterController controller;
    Vector3 lastVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastVelocity = controller.velocity;
    }

    Vector3 ScaleDirectionVector(Vector3 direction)
    {
        return new Vector3(
            direction.x,
            0,
            direction.z
        ).normalized * MovementSpeed * Time.deltaTime;
    }

    void Move()
    {
        Vector3 moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
        controller.SimpleMove(moveVector);
    }

    void RotateToVelocity()
    {
        Vector3 lookAt = transform.position + controller.velocity.normalized;
        Vector3 targetPosition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        if (targetPosition - transform.position != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, RotationSpeed * Time.deltaTime);
        }

    }

    Vector3 CalculateTilt(Vector3 acceleration)
    {
        acceleration.y = 0;
        Vector3 tiltAxis = Vector3.Cross(acceleration, Vector3.up);
        float angle = Mathf.Clamp(-acceleration.magnitude / 2, -TiltAngle, TiltAngle);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
        return targetRotation.eulerAngles;
    }

    void TiltToAcceleration()
    {
        Vector3 acceleration = (controller.velocity - lastVelocity) / Time.deltaTime;
        Vector3 tilt = CalculateTilt(acceleration);
        Quaternion targetRotation = Quaternion.Euler(tilt);
        playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        Move();
        RotateToVelocity();
        TiltToAcceleration();
        lastVelocity = controller.velocity;
    }
}