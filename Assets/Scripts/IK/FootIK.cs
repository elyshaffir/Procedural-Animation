using UnityEngine;

namespace ProceduralAnimation.IK
{
    class FootIK : IKLimb
    {

        public static FootIK CreateComponent(GameObject gameObject, string name, Transform parent, Vector3 polePosition)
        {
            FootIK component = CreateComponent<FootIK>(gameObject, name, parent, Vector3.zero, polePosition);
            return component;
        }

        void Awake()
        {
            chainLength = 2;
        }

        protected override void ResolveTargetRotation()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, LayerMask.NameToLayer("Ground")))
            {
                target.rotation = Quaternion.FromToRotation(playerTransform.up, hit.normal) * playerTransform.rotation;
                target.position = hit.point;
                target.position += new Vector3(0, 1f / 3, 0); // assuming target is sphere with radius 1
            }
        }
    }
}