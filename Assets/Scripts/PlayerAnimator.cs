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
        const string AirTimeVariable = "AirTime";
        const string ForwardVariable = "Forward";
        const string RightVariable = "Right";
        const string RollTimeVariable = "RollTime";

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
        float effort = 0.7f;
        float effortDifference = 0.01f;

        void Start()
        {
            momentumHandler = GetComponent<MomentumMovement>();
        }

        void Update()
        {
            SetAirTime();
            SetDown();
            SetRoll();
            SetMovement();
            SetIdle();
        }

        void SetIdle()
        {
            effort += effortDifference;
            if (effort >= 1 || effort <= 0.7f)
            {
                effort = Mathf.Round(effort * 10) / 10;
                effortDifference *= -1;
            }
            animator.SetFloat(EffortVariable, effort);
        }

        void SetAirTime()
        {
            if (momentumHandler.IsGrounded())
            {
                airTime = Mathf.Lerp(airTime, 0, AirTimeStep);
                floatStep = momentumHandler.GetHorizontalSpeed() / FloatStepDivider;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetHorizontalSpeed()));
            }
            else
            {
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.GetHorizontalSpeed()));
                airTime += AirTimeStep;
                airTime = Mathf.Clamp01(airTime);
            }
            animator.SetFloat(AirTimeVariable, airTime);
        }

        void SetDown()
        {
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

        void SetRoll()
        {
            float[] rollVariables = momentumHandler.GetRollVariables();
            animator.SetFloat(ForwardVariable, rollVariables[0]);
            animator.SetFloat(RightVariable, rollVariables[1]);
            animator.SetFloat(RollTimeVariable, rollVariables[2]);
        }

        void SetMovement()
        {
            if (momentumHandler.IsGrounded())
            {
                runprogress += floatStep;
                SetFloatSmooth(RunProgressVariable, runprogress);
                runflip += floatStep;
                SetFloatSmooth(MovementRunFlipVariable, runflip);
            }
        }

        void SetFloatSmooth(string name, float value)
        {
            animator.SetFloat(name, smoothCurve.Evaluate(value));
        }
    }
}