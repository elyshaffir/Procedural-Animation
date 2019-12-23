using UnityEngine;

[RequireComponent(typeof(CharacterController))]
class MomentumMovement : MonoBehaviour
{
    public GameObject playerCamera;

    CharacterController controller;
    float speed = 400f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
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

    void MoveAccordingToCamera()
    {
        Vector3 moveVector = ScaleDirectionVector(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += ScaleDirectionVector(playerCamera.transform.right) * Input.GetAxis("Horizontal");
        moveVector *= speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
    }

    void RotateAccordingToVelocity()
    {
        Vector3 lookAt = transform.position + controller.velocity.normalized;
        Vector3 targetPostition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        if (targetPostition - transform.position != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(targetPostition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 500 * Time.deltaTime);
        }

    }

    void Update()
    {
        MoveAccordingToCamera();
        RotateAccordingToVelocity();
    }
}