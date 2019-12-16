using UnityEngine;

class PlayerMovement : MonoBehaviour
{

    float VelocityLimit = 50;
    float AccelerationRate = .5f;
    float DecelerationRate = .1f;

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
            acceleration += transform.forward * AccelerationRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            acceleration += -transform.right * AccelerationRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            acceleration += -transform.forward * AccelerationRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            acceleration += transform.right * AccelerationRate * Time.deltaTime;
        }
        velocity += acceleration;
    }

    void HandleDeceleration()
    {
        Vector3 deceleration = new Vector3(
            System.Math.Sign(velocity.x),
            System.Math.Sign(velocity.y),
            System.Math.Sign(velocity.z)) * DecelerationRate;
        Debug.Log(deceleration.ToString());
        velocity -= deceleration / 2;
    }

    void HandleVelocity()
    {
        HandleAcceleration();
        HandleDeceleration();
        transform.position += velocity;
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
