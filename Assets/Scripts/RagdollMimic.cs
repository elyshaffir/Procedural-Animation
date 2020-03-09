using UnityEngine;

namespace ProceduralAnimation
{
    class RagdollMimic : MonoBehaviour
    {
        /*
        * We need to unite models into one since pose matching is a thing, unless it can be bypassed
            -   Pose matching could be done by trying to interpolate AFTER the physics affect the limb
                --  the same tecnique could be applied to both animations and keyframes
                --  if this works for animation, it is possible to leave it always in a state of animation matching, this way the model
                    will be interactive with physics
            -   Bone softness could be implemented by overshooting to the position of it in animation
				-- using the MoveTowards function is possible

		* Pose matching could be the same as putting an animator on a ragdoll - and let it fight itself.

		* Maybe, if all poses had spine at the exact same place, and just the rest of the bones move in relation to it, it would work.
			- But then animations would look funky
		
		* Maybe just don't move the hips when flailing?
        */
        const float LerpTime = 1000f;

        public GameObject mimicModel;
        public GameObject animatedModel;
        public Transform mimicObjectToFollow; // change to mimicTransformToFollow

        /* TEMPORARY */
        [Header("Temporary Variables")]
        public Transform[] mimicPoseMatchingTransforms;
        public Transform[] animatedPoseMatchingTransforms;
        public Transform animatedObjectToFollow;
        public LayerMask ground;
        /* YRAROPMET */

        bool physicsActive = false;
        Vector3 lastHipsPosition;

        void Start()
        {
            SetMimicPhysics(mimicModel.transform, physicsActive);
            UpdateLastHipsPosition();
        }

        void FixedUpdate()
        {
            if (!physicsActive)
            {
                Mimic(animatedModel.transform, mimicModel.transform);
            }
            else
            {
                Debug.Log("Ragdolling");
                animatedModel.transform.position += mimicObjectToFollow.position - lastHipsPosition; // the hips move in the animation -- nope, animator disabled.                
                animatedModel.transform.rotation = mimicObjectToFollow.rotation;

                for (int i = 0; i < mimicPoseMatchingTransforms.Length; i++)
                {
                    Transform animatedPoseMatchingTransform = animatedPoseMatchingTransforms[i];
                    Transform mimicPoseMatchingTranform = mimicPoseMatchingTransforms[i];
                    Poop(animatedPoseMatchingTransform, mimicPoseMatchingTranform);
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                physicsActive = !physicsActive;
                SetMimicPhysics(mimicModel.transform, physicsActive);
                SetPhysics(animatedModel.GetComponent<Rigidbody>(), !physicsActive);
            }
            UpdateLastHipsPosition();
        }

        void Poop(Transform currentAnimated, Transform currentMimic)
        {
            Rigidbody rb;
            if (currentMimic.TryGetComponent<Rigidbody>(out rb) || currentMimic.name == "Player Ragdoll") // Nasty
            {
                if (!Physics.CheckSphere(currentMimic.position, .6f, ground))
                // This prevents shaking on the floor! find the optimal number
                //	- Perhaps make it dependant on the velocity?
                {
                    currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, 10 * Time.deltaTime);
                    currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, 10 * Time.deltaTime);
                }
            }
            for (int i = 0; i < currentMimic.childCount; i++)
            {
                Poop(currentAnimated.GetChild(i), currentMimic.GetChild(i));
            }
        }

        void UpdateLastHipsPosition()
        {
            lastHipsPosition = mimicObjectToFollow.position;
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

        static void Mimic(Transform currentAnimated, Transform currentMimic)
        {
            Rigidbody rb;
            if (currentMimic.TryGetComponent<Rigidbody>(out rb) || currentMimic.name == "Player Ragdoll") // Nasty
            {
                currentMimic.localPosition = Vector3.Lerp(currentMimic.localPosition, currentAnimated.localPosition, LerpTime * Time.deltaTime);
                currentMimic.localRotation = Quaternion.Lerp(currentMimic.localRotation, currentAnimated.localRotation, LerpTime * Time.deltaTime);
            }
            for (int i = 0; i < currentMimic.childCount; i++)
            {
                Mimic(currentAnimated.GetChild(i), currentMimic.GetChild(i));
            }
        }
    }
}
