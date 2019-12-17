using UnityEngine;

class MomentumMovement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public float jumpForce;

    Rigidbody body;
    float vertical;
    float horizontal;
    bool isGrounded;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Jump") > 0)
        {
            if (isGrounded)
            {
                body.AddForce(transform.up * jumpForce);
            }
        }
        Vector3 velocity = (transform.forward * vertical) * movementSpeed * Time.fixedDeltaTime;
        velocity.y = body.velocity.y;
        body.velocity = velocity;
        transform.Rotate((transform.up * horizontal) * rotationSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag == ("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.root.tag == ("Ground"))
        {
            isGrounded = false;
        }
    }
}