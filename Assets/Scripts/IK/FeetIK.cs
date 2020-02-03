using UnityEngine;

public class FeetIK : MonoBehaviour
{
    const float DistanceToGround = 0.106f;

    public LayerMask ground;

    Animator animator;

    void Start()
    {
        Debug.Log("Change -ve3.up to ve3.down");
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    void OnAnimatorIK(int layerIndex)
    {
        SetFootIK(AvatarIKGoal.RightFoot, "RunFlip");
        SetFootIK(AvatarIKGoal.LeftFoot, "RunProgress");
    }

    void SetFootIK(AvatarIKGoal foot, string animatorWeightVariable)
    {
        // animator.SetIKPositionWeight(foot, animator.GetFloat(animatorWeightVariable));
        // animator.SetIKRotationWeight(foot, animator.GetFloat(animatorWeightVariable));
        animator.SetIKPositionWeight(foot, 1f);
        animator.SetIKRotationWeight(foot, 1f);

        RaycastHit hit;
        Ray ray = new Ray(animator.GetIKPosition(foot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, ground))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += DistanceToGround;
            animator.SetIKPosition(foot, footPosition);
            animator.SetIKRotation(foot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
        }
    }
}
