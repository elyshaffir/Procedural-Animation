using UnityEngine;

namespace ProceduralAnimation.IK
{
    class SpineIK : IKBehaviour
    {
        void LateUpdate()
        {
            // works only without animator
            // if you put another child under "spine" and attatch to it the HandIK script, it works in the way where
            // the more you offset the child from the original "spine" it affects the model more, and works properly with animator.
            // -- but: the movement is also extremely weird.
            // IKs that affect each other work. for example, a HandIK on spine6 with chainLength 6 works just fine with hand and leg IKs.
            // HandIK on spine with chainLength 1 works, but in a weird way as well.            
            transform.position = Vector3.Lerp(transform.position, target.position, snapBackStrength);
            // transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, snapBackStrength);
        }
    }
}