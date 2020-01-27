using UnityEngine;

namespace ProceduralAnimation
{
    class MomentumMovement : MonoBehaviour
    {
        const float MovementSpeedRunning = 1000f;
        const float MaxSpeedRunning = 20f;
        const float MovementSpeedWalking = MovementSpeedRunning / 1.5f;
        const float MaxSpeedWalking = MaxSpeedRunning / 1.5f;
        const float MovementSpeedCrouched = MovementSpeedRunning / 3;
        const float MaxSpeedCrouched = MaxSpeedRunning / 3;
        const float MaxGroundAngle = 50f;
        const float SlowDownRate = 2;
        const float RotationSpeed = 5f;
        const float TiltAngle = 15f;

        public GameObject playerCamera;
        public GameObject playerModel;
        public LayerMask ground;

        Rigidbody rb;
        bool grounded;
        bool crouching;
        float movementSpeed;
        float maxMovementSpeed;
        float forceAngle;
        Vector3 movementForce;

        public bool IsGrounded()
        {
            return grounded;
        }

        public bool IsCrouching()
        {
            return crouching;
        }

        public float GetHorizontalSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            return horizontalVelocity.magnitude / MaxSpeedRunning; // This should include rotation as well
        }

        public float GetVerticalSpeed()
        {
            return rb.velocity.y;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            CalculateMovement();
            Move();
            RotateToVelocity();
            TiltToAcceleration();
        }

        Vector3 ScaleDirectionVector(Vector3 direction)
        {
            return ToMovementSpeed(new Vector3(direction.x, 0, direction.z));
        }

        Vector3 ToMovementSpeed(Vector3 vector)
        {
            return vector.normalized * Time.deltaTime * movementSpeed;
        }

        void CalculateMovement()
        {
            RaycastHit hit;
            grounded = Physics.Raycast(transform.position, -Vector3.up, out hit, 1.5f, ground);
            forceAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (grounded)
            {
                Vector3 movementOnSurface = Vector3.Cross(hit.normal, Vector3.Cross(CalculateMovementVectors(), Vector3.up));
                Vector3 verticalMovement = Vector3.zero;
                movementForce = ToMovementSpeed(movementOnSurface + verticalMovement);
            }
        }

        Vector3 CalculateMovementVectors()
        {
            HandleSpeeds();
            Vector3 moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
            moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
            return moveVector;
        }

        void HandleSpeeds()
        {
            crouching = Input.GetKey(KeyCode.LeftControl); // Make sure there is downwards momentum added here, and upwards momentum upon jumping
            float targetMaxSpeed;
            if (crouching)
            {
                movementSpeed = MovementSpeedCrouched;
                targetMaxSpeed = MaxSpeedCrouched;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    movementSpeed = MovementSpeedRunning;
                    targetMaxSpeed = MaxSpeedRunning;
                }
                else
                {
                    movementSpeed = MovementSpeedWalking;
                    targetMaxSpeed = MaxSpeedWalking;
                }
            }
            maxMovementSpeed = Mathf.Lerp(maxMovementSpeed, targetMaxSpeed, 0.1f);
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
                if (Input.GetKey(KeyCode.Space))
                {
                    Debug.Log("a");
                    rb.AddForce(Vector3.up * 50);
                }
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxMovementSpeed);
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
            float angle = Mathf.Clamp(-movementForce.magnitude * GetHorizontalSpeed(), -TiltAngle, TiltAngle);
            Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
            return targetRotation.eulerAngles;
        }
    }
}