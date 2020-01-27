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

        Vector3 lastPosition;
        float floatStep;
        float forward;
        float movementSide = 1;
        float down;
        float currentDownVelocity;
        float targetDown;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
            lastPosition = transform.position;
            animator.SetFloat(EffortVariable, 0.7f);
        }

        void FixedUpdate()
        {
            if (momentumHandler.IsGrounded())
            {
                floatStep = (transform.position - lastPosition).magnitude / FloatStepDivider;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetSpeed()));
                SetMovementVariables();
                SetDownVariables();
            }
            lastPosition = transform.position;
        }

        void SetDownVariables()
        {
            targetDown = momentumHandler.IsCrouching() ? 1 - DownTargetMargin : DownTargetMargin;
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
            movementSide += floatStep;
            SetFloatSmooth(MovementSideVariable, movementSide);
        }

        void SetFloatSmooth(string name, float value)
        {
            animator.SetFloat(name, smoothCurve.Evaluate(value));
        }
    }
}