using UnityEngine;

namespace ProceduralAnimation
{
    class RagdollMimic : MonoBehaviour
    {
        // "Preserve whatever pose he was in by applying joint constraints"
        const float MimicLerpTime = 1000f;
        const float PoseMatchingLerpTime = 10f;
        /* 
		* This prevents shaking on the floor! find the optimal number
        	- Perhaps make it dependant on the velocity?
		*/
        const float MimicSphereRadius = 0.6f;

        public GameObject mimicModel;
        public GameObject animatedModel;
        public Transform mimicRoot;
        public Transform animatedRoot;
        public LayerMask ground;

        bool physicsActive = false;
        Vector3 lastHipsPosition;

        void Start()
        {
            SetMimicPhysics(mimicModel.transform, physicsActive);
        }

        void FixedUpdate()
        {
            if (physicsActive)
            {
                animatedModel.transform.position += mimicRoot.position - lastHipsPosition;
                for (int i = 0; i < animatedRoot.childCount; i++)
                {
                    Mimic(animatedRoot.GetChild(i), mimicRoot.GetChild(i), true);
                }
            }
            lastHipsPosition = mimicRoot.position;
        }

        void LateUpdate()
        {
            if (!physicsActive)
            {
                Mimic(animatedModel.transform, mimicModel.transform, false);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                physicsActive = !physicsActive;
                SetMimicPhysics(mimicModel.transform, physicsActive);
                SetPhysics(animatedModel.GetComponent<Rigidbody>(), !physicsActive);
            }
        }

        void Mimic(Transform currentAnimated, Transform currentMimic, bool poseMatch)
        {
            if (poseMatch)
            {
                if (
                    (currentMimic.TryGetComponent<Rigidbody>(out Rigidbody rb) || currentMimic.name == "Player Ragdoll") &&
                    !Physics.CheckSphere(currentMimic.position, MimicSphereRadius, ground)
                    )
                {
                    currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, PoseMatchingLerpTime * Time.deltaTime);
                    currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, PoseMatchingLerpTime * Time.deltaTime);
                }
            }
            else
            {
                // currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, MimicLerpTime * Time.deltaTime);
                // currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, MimicLerpTime * Time.deltaTime);
                // MimicLerpTime not used
                currentMimic.localPosition = currentAnimated.localPosition;
                currentMimic.localRotation = currentAnimated.localRotation;
            }

            for (int i = 0; i < currentMimic.childCount; i++)
            {
                Mimic(currentAnimated.GetChild(i), currentMimic.GetChild(i), poseMatch);
            }
        }

        void SetMimicPhysics(Transform current, bool active)
        {
            if (current.TryGetComponent<Rigidbody>(out Rigidbody currentRigidbody))
            {
                SetPhysics(currentRigidbody, active);
                if (active)
                {
                    currentRigidbody.velocity = animatedModel.GetComponent<Rigidbody>().velocity;
                }
            }
            for (int i = 0; i < current.childCount; i++)
            {
                SetMimicPhysics(current.GetChild(i), active);
            }
        }

        static void SetPhysics(Rigidbody rigidbody, bool active)
        {
            rigidbody.isKinematic = !active;
            rigidbody.detectCollisions = active;
        }
    }
}
