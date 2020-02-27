using UnityEngine;

namespace ProceduralAnimation.IK
{
    class IKLimb : FabrikEndBone
    {
        protected Transform playerTransform; // as of now, handIK doesn't need this - although that might change.

        protected override void ResolveTargetRotation() { }

        protected static T CreateComponent<T>(GameObject gameObject, string name, Transform parent, Vector3 localTargetPosition, Vector3 localPolePosition) where T : IKLimb
        {
            T component = CreateComponent<T>(gameObject, name, parent, localTargetPosition);
            component.playerTransform = gameObject.transform;
            component.pole = new GameObject(name + " Pole").transform;
            component.pole.parent = parent;
            component.pole.localPosition = localPolePosition;
            component.Init();
            return component;
        }
    }
}