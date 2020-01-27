using UnityEngine;

namespace ProceduralAnimation
{
    class PlayerCamera : MonoBehaviour
    {
        public GameObject player;
        public LayerMask solidObjects;

        const float HorizontalSpeed = 100.0f;
        const float VerticalSpeed = 100.0f;
        const float MinPitch = -20f;
        const float MaxPitch = 90;
        const float DistanceFromCamera = 3.75f;
        const float YOffset = 0f;

        CursorLockMode wantedMode;
        bool mouseActive;
        float yaw;
        float pitch;

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
            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        void UpdateLocation()
        {
            Vector3 idealPosition = player.transform.position - transform.forward * DistanceFromCamera;
            idealPosition = new Vector3(idealPosition.x, idealPosition.y + YOffset, idealPosition.z);
            Vector3 directionToCamera = idealPosition - player.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, directionToCamera, out hit, DistanceFromCamera, solidObjects))
            {
                transform.position = player.transform.position + directionToCamera.normalized * hit.distance;
            }
            else
            {
                transform.position = idealPosition;
            }
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
}