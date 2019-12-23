using UnityEngine;

[RequireComponent(typeof(CharacterController))]
class MomentumMovement : MonoBehaviour
{
    public GameObject playerCamera;

    CharacterController controller;
    float speed = 200f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    Vector3 Sign(Vector3 vector)
    {
        return new Vector3(
            Mathf.Sign(vector.x),
            Mathf.Sign(vector.y),
            Mathf.Sign(vector.z)
        );
    }

    void MoveAccordingToCamera()
    {
        Vector3 moveVector = Sign(playerCamera.transform.forward) * Input.GetAxis("Vertical");
        moveVector += Sign(playerCamera.transform.right) * Input.GetAxis("Horizontal");
        moveVector *= speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
        Debug.Log(playerCamera.transform.forward);
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