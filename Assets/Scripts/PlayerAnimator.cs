using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(MomentumMovement))]
    class PlayerAnimator : MonoBehaviour
    {

        private const string EffortVariable = "Effort";
        private const string SpeedVariable = "Speed";
        private const string RunProgressVariable = "RunProgress";
        private const string FootVariable = "Foot";

        public Animator animator;
        public AnimationCurve curve;

        MomentumMovement momentumHandler;

        Vector3 lastPosition;
        float floatStep;
        float runProgress;
        float foot;
        float movementProgressMultiplier = 1;
        bool floatToSet = true;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
            lastPosition = transform.position;
            animator.SetFloat(EffortVariable, 0.7f);
        }

        void Update()
        {
            if (momentumHandler.isGrounded())
            {
                floatStep = (transform.position - lastPosition).magnitude / 3; // Currently, this is not affected by how far along the animaiton we are - and it should be            
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.getSpeed()));
                HandleMovementProgressMultiplier();
                SetMovementVariables();
            }
            lastPosition = transform.position;
        }

        private void SetMovementVariables()
        {
            if (floatToSet)
            {
                SetRunProgress();
            }
            else
            {
                SetFoot();
            }
        }

        void SetFoot()
        {
            foot += floatStep * movementProgressMultiplier;
            foot = Mathf.Clamp01(foot);
            if (foot == 1 || foot == 0)
            {
                floatToSet = true;
            }
            SetFloatInterpolated(FootVariable, foot);
        }

        void SetRunProgress()
        {
            runProgress += floatStep * movementProgressMultiplier;
            runProgress = Mathf.Clamp01(runProgress);
            if (runProgress == 1 || runProgress == 0)
            {
                floatToSet = false;
            }
            SetFloatInterpolated(RunProgressVariable, runProgress);
        }

        void HandleMovementProgressMultiplier()
        {
            if (Mathf.Abs(runProgress) == Mathf.Abs(foot))
            {
                movementProgressMultiplier *= -1;
            }
        }

        void SetFloatInterpolated(string name, float value)
        {
            animator.SetFloat(name, curve.Evaluate(value));
        }
    }
}