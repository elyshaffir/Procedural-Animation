using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    class RagdollManager : MonoBehaviour
    {
        /*
        * To unite all models into one, you can instantiate new models where needed
        
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

        * Remember to move only the bones that are joints

        * Must unite all models into one, it's an ugly headache to solve the re-positionning of every model on each transition
        */
        const string TransformToFollow = "spine";
        const float BoneLerp = 40f;

        public GameObject playerModel;
        public GameObject playerRagdoll;
        public GameObject playerTransitionModel;
        public GameObject playerCameraTarget;

        bool lerping = false;
        bool notFinishedYet = false;
        List<string> jointNames; // list of all rigidbody bones names

        void Awake()
        {
            jointNames = new List<string>();
            GetJointNames(playerRagdoll.transform);
        }

        void GetJointNames(Transform currentBone)
        {
            if (currentBone.GetComponent<Rigidbody>() != null)
            {
                jointNames.Add(currentBone.name);
            }
            for (int i = 0; i < currentBone.childCount; i++)
            {
                GetJointNames(currentBone.GetChild(i));
            }
        }

        // remove ".transform" from "parent.transform"
        // optimize all getters/setters of that type
        void Update()
        {
            SetCameraTarget();
            if (!lerping)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    StartRagdoll();
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    StartLerpingFromRagdoll();
                }
            }
            else
            {
                LerpFromRagdoll();
            }
        }

        void StartRagdoll()
        {
            playerModel.transform.parent.gameObject.SetActive(false);
            playerRagdoll.transform.parent.gameObject.SetActive(true);
            ToRagdoll(playerModel.transform, playerRagdoll.transform, playerModel.transform.parent.GetComponent<Rigidbody>());
        }

        void StartLerpingFromRagdoll()
        {
            playerRagdoll.transform.parent.gameObject.SetActive(false);
            playerTransitionModel.transform.parent.gameObject.SetActive(true);
            ToRagdoll(playerRagdoll.transform, playerTransitionModel.transform);
            lerping = true;
        }

        void LerpFromRagdoll()
        {
            notFinishedYet = false;
            Debug.Log("transitioning");
            FromRagdoll(playerModel.transform, playerTransitionModel.transform);
            if (!notFinishedYet)
            {
                lerping = false;
                playerModel.transform.parent.gameObject.SetActive(true);
                playerTransitionModel.transform.parent.gameObject.SetActive(false);
            }
        }

        void SetCameraTarget()
        {
            if (playerModel.activeInHierarchy)
            {
                playerCameraTarget.transform.position = playerModel.transform.Find(TransformToFollow).position;
            }
            else if (playerRagdoll.activeInHierarchy)
            {
                playerCameraTarget.transform.position = playerRagdoll.transform.Find(TransformToFollow).position;
            }
            else
            {
                playerCameraTarget.transform.position = playerTransitionModel.transform.Find(TransformToFollow).position;
            }
        }

        void FromRagdoll(Transform currentAtModel, Transform currentAtTransition) // unite with toRagdoll
        {
            try
            {
                if (jointNames.Contains(currentAtModel.name))
                {
                    if (currentAtModel.childCount != currentAtTransition.childCount)
                    {
                        throw new UnityException("The model and the ragdoll are not exactly the same!");
                    }

                    currentAtTransition.position = Vector3.Lerp(currentAtTransition.position, currentAtModel.position, BoneLerp * Time.deltaTime);
                    if (currentAtTransition.position != currentAtModel.position)
                    {
                        notFinishedYet = true;
                    }

                    currentAtTransition.rotation = Quaternion.Lerp(currentAtTransition.rotation, currentAtModel.rotation, BoneLerp * Time.deltaTime);

                    // Vector3 newEuler = Vector3.Lerp(currentAtTransition.rotation.eulerAngles, currentAtRagdoll.rotation.eulerAngles, BoneLerp);
                    // currentAtTransition.rotation = Quaternion.Euler(newEuler);
                    if (currentAtTransition.rotation != currentAtModel.rotation)
                    {
                        notFinishedYet = true;
                    }
                }
            }
            catch (System.NullReferenceException) { }
            finally
            {
                for (int i = 0; i < currentAtTransition.childCount; i++)
                {
                    FromRagdoll(currentAtModel.GetChild(i), currentAtTransition.GetChild(i));
                }
            }
        }

        void ToRagdoll(Transform currentAtModel, Transform currentAtRagdoll, Rigidbody playerRigidbody = null) // Prettify
        {
            if (currentAtModel.childCount != currentAtRagdoll.childCount)
            {
                throw new UnityException("The model and the ragdoll are not exactly the same!");
            }

            if (playerRigidbody != null)
            {
                Rigidbody ragdollRigidbody;
                if (currentAtRagdoll.TryGetComponent<Rigidbody>(out ragdollRigidbody))
                {
                    ragdollRigidbody.velocity = playerRigidbody.velocity;
                }
            }

            currentAtRagdoll.position = currentAtModel.position;
            currentAtRagdoll.rotation = currentAtModel.rotation;

            for (int i = 0; i < currentAtModel.childCount; i++)
            {
                ToRagdoll(currentAtModel.GetChild(i), currentAtRagdoll.GetChild(i), playerRigidbody);
            }
        }
    }
}