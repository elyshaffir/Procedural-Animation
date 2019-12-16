using UnityEngine;

class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    const float AccelerationRate = 4f;
    const float DecelerationRate = 1.1f;

    Vector3 velocity;

    void Awake()
    {
        velocity = Vector3.zero;
    }

    void HandleAcceleration()
    {
        Vector3 acceleration = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            acceleration += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            acceleration += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            acceleration += -transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            acceleration += transform.right;
        }
        velocity += acceleration * AccelerationRate * Time.deltaTime;
    }

    void HandleRotation()
    {
        transform.eulerAngles = new Vector3(0, playerCamera.GetComponent<PlayerCamera>().GetYaw(), 0);
    }

    void HandleVelocity()
    {
        HandleRotation();
        HandleAcceleration();
        transform.position += velocity;
        velocity /= DecelerationRate;
    }

    void Update()
    {
        HandleVelocity();
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1);
    }
}
