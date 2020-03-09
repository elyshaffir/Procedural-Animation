using UnityEngine;

namespace ProceduralAnimation
{
    class RagdollMimic : MonoBehaviour
    {
        /*
		* Bone softness could be implemented by overshooting to the position of it in animation
			- using the MoveTowards function is possible
        */

        const float MimicLerpTime = 1000f;
        const float PoseMatchingLerpTime = 10f;
        /* 
		* This prevents shaking on the floor! find the optimal number
        	- Perhaps make it dependant on the velocity?
		*/
        const float MimicSphereRadius = 0.6f;

        public GameObject mimicModel;
        public GameObject animatedModel;
        public Transform mimicTransformToFollow;
        public Transform[] mimicPoseMatchingTransforms;
        public Transform[] animatedPoseMatchingTransforms;
        public LayerMask ground;

        bool physicsActive = false;
        Vector3 lastHipsPosition;

        void Start()
        {
            SetMimicPhysics(mimicModel.transform, physicsActive);
        }

        void FixedUpdate()
        {
            if (!physicsActive)
            {
                Mimic(animatedModel.transform, mimicModel.transform, false);
            }
            else
            {
                animatedModel.transform.position += mimicTransformToFollow.position - lastHipsPosition;
                for (int i = 0; i < mimicPoseMatchingTransforms.Length; i++)
                {
                    Transform animatedPoseMatchingTransform = animatedPoseMatchingTransforms[i];
                    Transform mimicPoseMatchingTranform = mimicPoseMatchingTransforms[i];
                    Mimic(animatedPoseMatchingTransform, mimicPoseMatchingTranform, true);
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                physicsActive = !physicsActive;
                SetMimicPhysics(mimicModel.transform, physicsActive);
                SetPhysics(animatedModel.GetComponent<Rigidbody>(), !physicsActive);
            }
            lastHipsPosition = mimicTransformToFollow.position;
        }

        void Mimic(Transform currentAnimated, Transform currentMimic, bool poseMatch)
        {
            Rigidbody rb;
            if (currentMimic.TryGetComponent<Rigidbody>(out rb) || currentMimic.name == "Player Ragdoll") // Nasty
            {
                if (poseMatch)
                {
                    if (!Physics.CheckSphere(currentMimic.position, MimicSphereRadius, ground))
                    {
                        currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, PoseMatchingLerpTime * Time.deltaTime);
                        currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, PoseMatchingLerpTime * Time.deltaTime);
                    }
                }
                else
                {
                    // Bone softness (overshooting probably) here
                    currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, MimicLerpTime * Time.deltaTime);
                    currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, MimicLerpTime * Time.deltaTime);
                }
            }
            for (int i = 0; i < currentMimic.childCount; i++)
            {
                Mimic(currentAnimated.GetChild(i), currentMimic.GetChild(i), poseMatch);
            }
        }

        void SetMimicPhysics(Transform current, bool active)
        {
            Rigidbody currentRigidbody;
            if (current.TryGetComponent<Rigidbody>(out currentRigidbody))
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
