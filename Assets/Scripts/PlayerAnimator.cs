using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(MomentumMovement))]
    class PlayerAnimator : MonoBehaviour
    {

        const string EffortVariable = "Effort";
        const string SpeedVariable = "Speed";
        const string RunProgressVariable = "RunProgress";
        const string MovementRunFlipVariable = "RunFlip";
        const string DownVariable = "Down";
        const float AirTimeStep = 0.1f;
        const float FloatStepDivider = 5f;
        const float DownTargetMargin = .2f;
        const float DownStiffness = 50f;
        const float DownDamping = 6f;
        const float DownThreshold = 0.01f;
        const float DownVelocityThreshold = 0.01f;

        public Animator animator;
        public AnimationCurve smoothCurve;

        MomentumMovement momentumHandler;


        float airTime;
        float floatStep;
        float runprogress;
        float runflip = 1;
        float down;
        float currentDownVelocity;
        float targetDown;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
            animator.SetFloat(EffortVariable, 0.7f);
        }

        void Update()
        {
            if (momentumHandler.IsGrounded())
            {
                airTime = Mathf.Lerp(airTime, 0, AirTimeStep);
                floatStep = momentumHandler.GetHorizontalSpeed() / FloatStepDivider;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetHorizontalSpeed()));
                SetMovementVariables();
            }
            else
            {
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetHorizontalSpeed()));
                airTime += AirTimeStep;
                airTime = Mathf.Clamp01(airTime);
            }
            SetDownVariables();
            SetRollVariables();
        }

        void SetDownVariables()
        {
            animator.SetFloat("AirTime", airTime);
            if (momentumHandler.IsCrouching())
            {
                targetDown = 1 - DownTargetMargin;
            }
            else
            {
                targetDown = DownTargetMargin - momentumHandler.GetVerticalSpeed();
            }
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

        void SetRollVariables()
        {
            float[] rollVariables = momentumHandler.GetRollVariables();
            animator.SetFloat("Forward", rollVariables[0]);
            animator.SetFloat("Right", rollVariables[1]);
            animator.SetFloat("RollTime", rollVariables[2]);
        }

        void SetMovementVariables()
        {
            runprogress += floatStep;
            SetFloatSmooth(RunProgressVariable, runprogress);
            runflip += floatStep;
            SetFloatSmooth(MovementRunFlipVariable, runflip);
        }

        void SetFloatSmooth(string name, float value)
        {
            animator.SetFloat(name, smoothCurve.Evaluate(value));
        }
    }
}