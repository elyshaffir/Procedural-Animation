using UnityEngine;

class HandIK : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform lookObj = null;
    public Transform spineObj;
    public GameObject playerSpine;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        playerSpine.transform.position = spineObj.position;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        // animator.GetBoneTransform(HumanBodyBones.Spine).transform.position = spineObj.position;
        // animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.LookRotation(transform.position - spineObj.position, Vector3.up));     
        // animator.MatchTarget(spineObj.position, spineObj.rotation, AvatarTarget.Body, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), 0);
        // SetLookAtWeight(float weight, float bodyWeight, float headWeight);        

        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look __target position__, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
