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
        */
        const float LerpTime = 10f;

        public GameObject mimicModel;
        public GameObject animatedModel;
        public Transform mimicObjectToFollow;

        bool physicsActive = false;

        void Start()
        {
            SetMimicPhysics(mimicModel.transform, physicsActive);
        }

        void FixedUpdate()
        {
            if (!physicsActive)
            {
                Mimic(animatedModel.transform, mimicModel.transform);
            }
            else
            {
                animatedModel.transform.position = mimicObjectToFollow.position;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                physicsActive = !physicsActive;
                SetMimicPhysics(mimicModel.transform, physicsActive);
                SetPhysics(animatedModel.GetComponent<Rigidbody>(), !physicsActive);
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

        static void Mimic(Transform currentAnimated, Transform currentMimic)
        {
            Rigidbody rb;
            if (currentMimic.TryGetComponent<Rigidbody>(out rb) || currentMimic.name == "Player Ragdoll") // Nasty
            {
                currentMimic.position = Vector3.Lerp(currentMimic.position, currentAnimated.position, LerpTime * Time.deltaTime);
                currentMimic.rotation = Quaternion.Lerp(currentMimic.rotation, currentAnimated.rotation, LerpTime * Time.deltaTime);
            }
            /*
                This loop assumes that if a currentAnimated were to have more children than currentMimic,
                they were irrelevant for the mimicking process.

                It also assumes that the relevant children for the mimicking process would be first in
                the hierarchy.
            */
            for (int i = 0; i < currentMimic.childCount; i++)
            {
                Mimic(currentAnimated.GetChild(i), currentMimic.GetChild(i));
            }
        }
    }
}
