using UnityEngine;

namespace ProceduralAnimation.IK
{
    class FootIK : FabrikEndBone
    {

        [Header("FootIK Parameters")]
        public LayerMask ground;
        public Transform playerTransform;

        protected override void ResolveTargetRotation()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
            {
                target.rotation = Quaternion.FromToRotation(playerTransform.up, hit.normal) * playerTransform.rotation;
                target.position = hit.point;
                target.position += new Vector3(0, target.lossyScale.y / 3, 0); // assuming target is sphere                
            }
        }
    }
}