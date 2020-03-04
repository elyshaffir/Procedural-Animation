using UnityEngine;

namespace ProceduralAnimation
{
    class RagdollMimic : MonoBehaviour
    {
        /*
        * We need to unite models into one since pose matching is a thing, unless it can be bypassed
            -   Pose matching could be done by trying to interpolate AFTER the physics affect the limb
                --  if this can be done, we can probably get rid of the playerTransitionModel
                --  the same tecnique could be applied to both animations and keyframes
                    --- provided that we can find a way to have the animation still run while the animated model is invisible
                --  if this works for animation, it is possible to leave it always in a state of animation matching, this way the model
                    will be interactive with physics
            -   Bone softness could be implemented by the speed at which the bone interpolates to the position of it in animation

        * When pressing H without having the ragdoll active, it could be a clue to the animation matching
            - The one model that will be seen by the user is that mimicking ragdoll, while there is another invisible model being animated 
                -- IK will affect the invisible animated model

        * Remember to move only the bones that are joints

        * Must unite all models into one, it's an ugly headache to solve the re-positionning of every model on each transition        
        */
        const float LerpTime = 10f;

        public GameObject mimicModel;
        public GameObject animatedModel;

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
            currentMimic.position = Vector3.Lerp(currentMimic.position, currentAnimated.position, LerpTime * Time.deltaTime);
            currentMimic.rotation = Quaternion.Lerp(currentMimic.rotation, currentAnimated.rotation, LerpTime * Time.deltaTime);

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
