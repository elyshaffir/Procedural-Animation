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

    void MoveAccordingToCamera()
    {
        Vector3 moveVector = playerCamera.transform.forward * Input.GetAxis("Vertical");
        moveVector += playerCamera.transform.right * Input.GetAxis("Horizontal");
        moveVector *= speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
    }

    void RotateAccordingToVelocity()
    {
        Vector3 lookAt = transform.position + controller.velocity.normalized;
        Vector3 targetPostition = new Vector3(lookAt.x, transform.position.y, lookAt.z);
        // transform.LookAt(targetPostition);
        Quaternion q = Quaternion.LookRotation(targetPostition - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 500 * Time.deltaTime);

    }

    void Update()
    {
        MoveAccordingToCamera();
        RotateAccordingToVelocity();
    }
}