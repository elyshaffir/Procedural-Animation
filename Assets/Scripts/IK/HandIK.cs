using UnityEngine;

namespace ProceduralAnimation.IK
{
    class HandIK : IKLimb
    {

        public static HandIK CreateComponent(GameObject gameObject, string name, Transform parent, Vector3 localTargetPosition, Vector3 localPolePosition)
        {
            return CreateComponent<HandIK>(gameObject, name, parent, localTargetPosition, localPolePosition);
        }

        void Awake()
        {
            chainLength = 3;
        }
    }
}