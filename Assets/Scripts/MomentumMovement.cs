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

    void Update()
    {
        Vector3 moveVector = playerCamera.transform.forward * Input.GetAxis("Vertical");
        moveVector += playerCamera.transform.right * Input.GetAxis("Horizontal");
        moveVector *= speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
    }
}