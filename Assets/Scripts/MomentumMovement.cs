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
        const float JumpForce = 50f;
        const float RollForce = 10f;
        const float RollSpeed = 0.05f;

        public GameObject playerCamera;
        public LayerMask ground;

        Rigidbody rb;
        bool grounded;
        bool crouching;
        float movementSpeed;
        float maxMovementSpeed;
        float forceAngle;
        Vector3 movementForce;
        Vector3 moveVector;
        bool preparingRoll = false;
        bool rolling = false;
        Vector3 rotationAxis;
        float rollProgress;

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

        public float[] GetRollVariables()
        {
            float forward = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(moveVector, transform.forward));
            float right = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(moveVector, transform.right));
            return new float[] { forward * rollProgress, right * rollProgress, rollProgress };
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            CalculateMovementVectors();
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
            grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.6f, ground);
            forceAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (grounded)
            {
                Vector3 movementOnSurface = Vector3.Cross(hit.normal, Vector3.Cross(moveVector, Vector3.up));
                Vector3 verticalMovement = Vector3.zero;
                movementForce = ToMovementSpeed(movementOnSurface + verticalMovement);
            }
        }

        void CalculateMovementVectors()
        {
            HandleSpeeds();
            moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
            moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
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
            if (preparingRoll)
            {
                PrepareToRoll();
            }
            else if (rolling)
            {
                Roll();
            }
            else
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
                        rb.AddForce(Vector3.up * JumpForce);
                    }
                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxMovementSpeed);
                }
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    rb.AddForce(moveVector.normalized * RollForce, ForceMode.Impulse);
                    preparingRoll = true;
                    rotationAxis = Vector3.Cross(Vector3.up, moveVector);
                }
            }
        }

        void Roll()
        {
            rollProgress -= RollSpeed;
            transform.Rotate(rotationAxis, 360 * RollSpeed, Space.World);
            if (rollProgress <= 0)
            {
                rollProgress = 0;
                rolling = false;
            }
        }

        void PrepareToRoll()
        {
            rollProgress += RollSpeed;
            if (rollProgress >= 1)
            {
                rollProgress = 1;
                rolling = true;
                preparingRoll = false;
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
            if (!rolling)
            {
                Vector3 tilt = CalculateTilt();
                Quaternion targetRotation = Quaternion.Euler(tilt);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
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