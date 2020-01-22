using System;
using UnityEngine;

[RequireComponent(typeof(MomentumMovement))]
class PlayerAnimator : MonoBehaviour
{

    private const string EffortVariable = "Effort";
    private const string SpeedVariable = "Speed";
    private const string RunProgressVariable = "RunProgress";
    private const string FootVariable = "Foot";

    public Animator animator;

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
            SetFloat(SpeedVariable, Mathf.Clamp01(momentumHandler.getSpeed()));
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
        SetFloat(FootVariable, foot);
    }

    void SetRunProgress()
    {
        runProgress += floatStep * movementProgressMultiplier;
        runProgress = Mathf.Clamp01(runProgress);
        if (runProgress == 1 || runProgress == 0)
        {
            floatToSet = false;
        }
        SetFloat(RunProgressVariable, runProgress);
    }

    void HandleMovementProgressMultiplier()
    {
        if (Mathf.Abs(runProgress) == Mathf.Abs(foot))
        {
            movementProgressMultiplier *= -1;
        }
    }

    void SetFloat(string name, float value) // This is probably where we implement interpolation manipulating
    {
        float currentValue = animator.GetFloat(name);
        // From here on out, we'll assume that value is clamped between 0 and 1
        // float dampTime = Mathf.Abs(currentValue - 0.5f); // so that it gets slower the closer you get to 0 and 1                
        animator.SetFloat(name, InterpolateCubic(value - floatStep, currentValue, value, .0f, 1f)); // Using damping might be helpful                
    }

    public float InterpolateCubic(float v0, float v1, float v2, float v3, float x)
    {
        float p = (v3 - v2) - (v0 - v1);
        float q = (v0 - v1) - p;
        float r = v2 - v0;
        float s = v1;
        return p * x * x * x + q * x * x + r * x + s;
    }
}
