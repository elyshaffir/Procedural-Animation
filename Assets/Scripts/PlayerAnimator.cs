using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(MomentumMovement))]
    class PlayerAnimator : MonoBehaviour
    {

        const string EffortVariable = "Effort";
        const string SpeedVariable = "Speed";
        const string ForwardVariable = "Forward";
        const string MovementSideVariable = "Side";
        const string DownVariable = "Down";
        const float FloatStepDivider = 5f;
        const float DownTargetMargin = .2f;
        const float DownStiffness = 50f;
        const float DownDamping = 6f;
        const float DownThreshold = 0.01f;
        const float DownVelocityThreshold = 0.01f;

        public Animator animator;
        public AnimationCurve smoothCurve;

        MomentumMovement momentumHandler;

        float floatStep;
        float forward;
        float side = 1;
        float down;
        float currentDownVelocity;
        float targetDown;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
            animator.SetFloat(EffortVariable, 0.7f);
        }

        void FixedUpdate()
        {
            animator.SetBool("Grounded", momentumHandler.IsGrounded());
            if (momentumHandler.IsGrounded())
            {
                floatStep = momentumHandler.GetHorizontalSpeed() / FloatStepDivider;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetHorizontalSpeed()));
                SetMovementVariables();
            }
            SetDownVariables();
        }

        void SetDownVariables()
        {
            targetDown = momentumHandler.IsCrouching() ? 1 - DownTargetMargin : DownTargetMargin - momentumHandler.GetVerticalSpeed(); // this can exceed the limits            
            targetDown = Mathf.Clamp01(targetDown);
            float dampingFactor = 1 - DownDamping * Time.fixedDeltaTime;
            float acceleration = (targetDown - down) * DownStiffness * Time.fixedDeltaTime;
            currentDownVelocity = currentDownVelocity * dampingFactor + acceleration;
            down += currentDownVelocity * Time.fixedDeltaTime;

            if (Mathf.Abs(down - targetDown) < DownThreshold && Mathf.Abs(currentDownVelocity) < DownVelocityThreshold)
            {
                down = targetDown;
                currentDownVelocity = 0f;
            }
            animator.SetFloat(DownVariable, down);
        }

        void SetMovementVariables()
        {
            forward += floatStep;
            SetFloatSmooth(ForwardVariable, forward);
            side += floatStep;
            SetFloatSmooth(MovementSideVariable, side);
        }

        void SetFloatSmooth(string name, float value)
        {
            animator.SetFloat(name, smoothCurve.Evaluate(value));
        }
    }
}