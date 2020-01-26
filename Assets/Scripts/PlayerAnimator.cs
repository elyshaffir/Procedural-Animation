using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(MomentumMovement))]
    class PlayerAnimator : MonoBehaviour
    {

        private const string EffortVariable = "Effort";
        private const string SpeedVariable = "Speed";
        private const string RunProgressVariable = "RunProgress";
        private const string MovementSideVariable = "MovementSide";
        private const string CrouchVariable = "Crouch";
        private const float FloatStepDivider = 5;
        private const float CrouchSpeed = 0.02f;

        public Animator animator;
        public AnimationCurve smoothCurve;

        MomentumMovement momentumHandler;

        Vector3 lastPosition;
        float floatStep;
        float runProgress;
        float movementSide = 1;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
            lastPosition = transform.position;
            animator.SetFloat(EffortVariable, 0.7f);
        }

        void Update()
        {
            if (momentumHandler.IsGrounded())
            {
                floatStep = (transform.position - lastPosition).magnitude / FloatStepDivider;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetSpeed()));
                SetMovementVariables();
                SetCrouchingVariables();
            }
            lastPosition = transform.position;
        }

        void SetCrouchingVariables()
        {
            if (momentumHandler.IsCrouching())
            {
                animator.SetFloat(CrouchVariable, Mathf.Clamp01(animator.GetFloat(CrouchVariable) + CrouchSpeed));
            }
            else
            {
                animator.SetFloat(CrouchVariable, Mathf.Clamp01(animator.GetFloat(CrouchVariable) - CrouchSpeed));
            }
        }

        void SetMovementVariables()
        {
            runProgress += floatStep;
            SetFloatInterpolated(RunProgressVariable, runProgress);
            movementSide += floatStep;
            SetFloatInterpolated(MovementSideVariable, movementSide);
        }

        void SetFloatInterpolated(string name, float value)
        {
            animator.SetFloat(name, smoothCurve.Evaluate(value));
        }
    }
}