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
        float foot = 1;

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
                floatStep = (transform.position - lastPosition).magnitude / 5;
                animator.SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.getSpeed())); // To small                
                SetMovementVariables();
            }
            lastPosition = transform.position;
        }

        private void SetMovementVariables()
        {
            runProgress += floatStep;
            SetFloatInterpolated(RunProgressVariable, runProgress);
            foot += floatStep;
            SetFloatInterpolated(FootVariable, foot);
        }

        void SetFloatInterpolated(string name, float value)
        {
            animator.SetFloat(name, curve.Evaluate(value));
        }
    }
}