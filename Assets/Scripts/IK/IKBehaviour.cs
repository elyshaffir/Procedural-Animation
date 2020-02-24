using UnityEngine;

namespace ProceduralAnimation.IK
{
    abstract class IKBehaviour : MonoBehaviour
    {
        public Transform target;
        [Range(0, 1)]
        public float snapBackStrength = 1f;
    }
}