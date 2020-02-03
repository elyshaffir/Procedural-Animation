using UnityEngine;

class IK : MonoBehaviour
{
    /*
    void FABRIK()
    {
        while (abs(endEffectorPosition - goalPosition) > EPS)
        {
            FinalToRoot(); // PartOne
            RootToFinal(); // PartTwo
        }
    }

    // Part One
    void FinalToRoot()
    {
        currentGoal = goalPosition;
        currentLimb = finalLimb;
        while (currentLimb != NULL)
        {
            currentLimb.rotation = RotFromTo(Vector.UP,
                currentGoal — currentLimb.inboardPosition);
            currentLimb.outboardPosition = currentGoal;
            currentGoal = currentLimb.inboardPosition;
            currentLimb = currentLimb->inboardLimb;
        }
    }
    // Part Two
    void RootToFinal()
    {
        currentInboardPosition = rootLimb.inboardPosition;
        currentLimb = rootLimb;
        while (currentLimb != NULL)
        {
            currentLimb.inboardPosition = currentInboardPosition;
            currentInboardPosition = currentLimb.outboardPosition;
            currentLimb = currentLimb->outboardLimb;
        }
    }
    */
}
