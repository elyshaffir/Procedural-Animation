using UnityEngine;

class PlayerMovement : MonoBehaviour
{

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

    void HandleVelocity()
    {
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
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
