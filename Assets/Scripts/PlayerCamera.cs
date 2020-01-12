using UnityEngine;

class PlayerCamera : MonoBehaviour
{
    public GameObject player;

    const float HorizontalSpeed = 100.0f;
    const float VerticalSpeed = 100.0f;
    const float DistanceFromCamera = 3.75f;
    const float YOffset = 0;

    CursorLockMode wantedMode;
    bool mouseActive = true;
    float yaw = 0.0f;
    float pitch = 0.0f;

    void Start()
    {
        wantedMode = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        SetCursorState();
        HandleCameraLook(); // Do it so that if the camera isn't being moved, by deault it would look to where the player is going
    }

    void HandleCameraLook()
    {
        if (wantedMode != CursorLockMode.None)
        {
            UpdateRotation();
            UpdateLocation();
        }
    }

    void UpdateRotation()
    {
        yaw += HorizontalSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        pitch -= VerticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void UpdateLocation()
    {
        transform.position = player.transform.position - transform.forward * DistanceFromCamera;
        transform.position = new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z);
    }

    void SetCursorState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = wantedMode = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            wantedMode = CursorLockMode.Locked;
        }

        Cursor.lockState = wantedMode;
        Cursor.visible = (CursorLockMode.Locked != wantedMode);
    }

    public float GetYaw()
    {
        return yaw;
    }
}
