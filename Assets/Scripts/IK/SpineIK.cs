using UnityEngine;

namespace ProceduralAnimation.IK
{
    class SpineIK : IKBone
    {
        Vector3 lastTargetPosition; // move the IK by the difference in distance from the last frame, add to the position -- dont override it.

        public static SpineIK CreateComponent(GameObject gameObject, string name, Transform parent)
        {
            return CreateComponent<SpineIK>(gameObject, name, parent, Vector3.zero);
        }

        void Start()
        {
            lastTargetPosition = target.position;
        }


        void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, snapBackStrength);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, snapBackStrength);
        }
    }
}