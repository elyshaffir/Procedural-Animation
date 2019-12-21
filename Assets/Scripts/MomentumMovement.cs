using UnityEngine;

[RequireComponent(typeof(CharacterController))]
class MomentumMovement : MonoBehaviour
{
    const float Speed = 400f;

    public GameObject playerCamera;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 moveVector = playerCamera.transform.forward * Input.GetAxis("Vertical");
        moveVector += playerCamera.transform.right * Input.GetAxis("Horizontal");
        moveVector *= Speed * Time.deltaTime;
        controller.SimpleMove(moveVector);
    }
}