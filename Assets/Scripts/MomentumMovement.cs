using UnityEngine;

namespace ProceduralAnimation
{
    class MomentumMovement : MonoBehaviour
    {
        const float MovementSpeed = 1400f;
        const float MaxGroundAngle = 50f;
        const float MaxVelocity = 20f;
        const float SlowDownRate = 2;
        const float RotationSpeed = 5f;
        const float TiltAngle = 15f;

        public GameObject playerCamera;
        public GameObject playerModel;
        public LayerMask ground;

        Rigidbody rb;
        bool grounded;
        float forceAngle;
        Vector3 movementForce;

        public bool isGrounded()
        {
            return grounded;
        }

        public float getSpeed()
        {
            return rb.velocity.magnitude / 30;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            CalculateGrounded();
            Move();
            RotateToVelocity();
            TiltToAcceleration();
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
                Vector3 extraForceDirection = Vector3.Cross(hit.normal, Vector3.Cross(CalculateMoveVector(), Vector3.up));
                movementForce = extraForceDirection.normalized * MovementSpeed * Time.deltaTime;
            }
        }

        Vector3 CalculateMoveVector()
        {
            Vector3 moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
            moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
            return moveVector;
        }

        void Move()
        {

            if (grounded && forceAngle < MaxGroundAngle)
            {
                if (movementForce == Vector3.zero)
                {
                    movementForce = -rb.velocity * SlowDownRate;
                }
                rb.AddForce(movementForce);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
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
            Vector3 tilt = CalculateTilt();
            Quaternion targetRotation = Quaternion.Euler(tilt);
            playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        Vector3 CalculateTilt()
        {
            Vector3 tiltAxis = Vector3.Cross(movementForce, Vector3.up);
            float angle = Mathf.Clamp(-movementForce.magnitude * getSpeed(), -TiltAngle, TiltAngle);
            Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
            return targetRotation.eulerAngles;
        }
    }
}