using System.Collections.Generic;
using UnityEngine;


class MomentumMovement : MonoBehaviour
{

    const float MOVEMENT_SPEED_LIMIT = 30;

    public float movementSpeed;
    public float rotationSpeed;
    public float jumpForce;

    Rigidbody body;
    float vertical;
    float horizontal;
    HashSet<GameObject> environmentTouching;

    void Awake()
    {
        environmentTouching = new HashSet<GameObject>();
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        if (environmentTouching.Count > 0)
        {
            if (Input.GetAxis("Jump") > 0)
            {
                body.AddForce(transform.up * jumpForce);
            }
            Vector3 velocity = (transform.forward * vertical) * movementSpeed * Time.fixedDeltaTime;
            velocity.y = body.velocity.y;
            body.velocity += velocity;
        }
        Vector3 angularVelocity = (transform.up * horizontal) * rotationSpeed * Time.fixedDeltaTime;
        angularVelocity.x = body.angularVelocity.y;
        angularVelocity.z = body.angularVelocity.z;
        body.angularVelocity += angularVelocity;
        if (body.velocity.magnitude > MOVEMENT_SPEED_LIMIT)
        {
            body.velocity = Vector3.ClampMagnitude(body.velocity, MOVEMENT_SPEED_LIMIT);
        }
        if (body.angularVelocity.magnitude > MOVEMENT_SPEED_LIMIT)
        {
            body.angularVelocity = Vector3.ClampMagnitude(body.angularVelocity, MOVEMENT_SPEED_LIMIT);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag == ("Environment"))
        {
            environmentTouching.Add(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.root.tag == ("Environment"))
        {
            environmentTouching.Remove(collision.gameObject);
        }
    }
}