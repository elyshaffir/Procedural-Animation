using UnityEngine;

public class FABRIKIKPlayerTest : FABRIK
{
    // public Transform sphere_right;
    public Transform sphere_left;

    public override void OnFABRIK()
    {
        float speed = 10000.0F;
        float step = Time.deltaTime * speed;

        // FABRIKChain right = GetEndChain("f_index.03.L_end_end_effector");
        // FABRIKChain left = GetEndChain("f_index.03.L_end_end_effector");

        // right.Target = Vector3.MoveTowards(right.EndEffector.Position, sphere_right.position, step);
        // left.Target = Vector3.MoveTowards(left.EndEffector.Position, sphere_left.position, step);
        // FABRIKChain left = GetEndChain("heel.02.L_end"); // The rig is not suitable for that, the end bones mostly do nothing        
        FABRIKChain left = GetEndChain("face_end"); // even on the neck it gets fucked up - but it seems to fuck up only in the first bone.
        // Another problem might be the calculation of bone length
        left.Target = Vector3.MoveTowards(left.EndEffector.Position, sphere_left.position, step);
    }
}
