using UnityEngine;

[RequireComponent(typeof(MomentumMovement))]
class PlayerAnimator : MonoBehaviour
{

    public Animator animator;

    MomentumMovement momentumHandler;

    Vector3 lastPosition;
    float floatStep;
    float runProgress;
    float foot;
    float multiplier = 1;
    bool floatToSet = true;

    void Start()
    {
        momentumHandler = GetComponent<MomentumMovement>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (momentumHandler.isGrounded())
        {
            floatStep = (transform.position - lastPosition).magnitude / 3;
            if (Mathf.Abs(runProgress) == Mathf.Abs(foot))
            {
                multiplier *= -1;
            }
            if (floatToSet)
            {
                runProgress += floatStep * multiplier;
                runProgress = Mathf.Clamp01(runProgress);
                if (runProgress == 1 || runProgress == 0)
                {
                    floatToSet = false;
                }
                animator.SetFloat("RunProgress", runProgress);
            }
            else
            {
                foot += floatStep * multiplier;
                foot = Mathf.Clamp01(foot);
                if (foot == 1 || foot == 0)
                {
                    floatToSet = true;
                }
                animator.SetFloat("Foot", foot);
            }
        }
        lastPosition = transform.position;
    }
}
