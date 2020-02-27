using UnityEngine;

namespace ProceduralAnimation.IK
{
    abstract class IKBone : MonoBehaviour
    {
        public Transform target;
        public float snapBackStrength = 1f; // ranged 1-0

        protected static T CreateComponent<T>(GameObject gameObject, string name, Transform parent, Vector3 localTargetPosition) where T : IKBone
        {
            T component = gameObject.AddComponent<T>();
            component.target = new GameObject(name + " Target").transform;
            component.target.parent = parent;
            component.target.localPosition = localTargetPosition;
            return component;
        }
    }
}