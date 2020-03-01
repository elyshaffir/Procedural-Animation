using UnityEditor;
using UnityEngine;

namespace ProceduralAnimation.IK
{
    abstract class FabrikEndBone : IKBone
    {
        public Transform pole;

        protected int chainLength;
        protected int iterations = 10;
        protected float delta = 0.001f;

        float[] boneLengths; // Target to origin
        float completeLength;
        Transform[] bones;
        Vector3[] positions;
        Vector3[] startDirections;
        Quaternion[] startBoneRotations;
        Quaternion startTargetRotation;
        Transform root;

        protected void Init()
        {
            bones = new Transform[chainLength + 1];
            positions = new Vector3[chainLength + 1];
            boneLengths = new float[chainLength];
            startDirections = new Vector3[chainLength + 1];
            startBoneRotations = new Quaternion[chainLength + 1];

            //find root
            root = transform;
            for (int i = 0; i <= chainLength; i++)
            {
                if (root == null)
                {
                    throw new UnityException("The chain value is longer than the ancestor chain!");
                }
                root = root.parent;
            }

            startTargetRotation = GetRotationRootSpace(target);

            //init data
            Transform current = transform;
            completeLength = 0;
            for (int i = bones.Length - 1; i >= 0; i--)
            {
                bones[i] = current;
                startBoneRotations[i] = GetRotationRootSpace(current);

                if (i == bones.Length - 1)
                {
                    //leaf
                    startDirections[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
                }
                else
                {
                    //mid bone
                    startDirections[i] = GetPositionRootSpace(bones[i + 1]) - GetPositionRootSpace(current);
                    boneLengths[i] = startDirections[i].magnitude;
                    completeLength += boneLengths[i];
                }

                current = current.parent;
            }
        }

        void LateUpdate()
        {
            ResolveTargetRotation();
            ResolveIK();
        }

        void ResolveIK()
        {
            if (boneLengths.Length != chainLength)
            {
                Init();
            }

            // Fabric

            //  root
            //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
            //   x--------------------x--------------------x---...

            //get position
            for (int i = 0; i < bones.Length; i++)
            {
                positions[i] = GetPositionRootSpace(bones[i]);
            }

            Vector3 targetPosition = GetPositionRootSpace(target);
            Quaternion targetRotation = GetRotationRootSpace(target);

            // 1st is possible to reach?
            if ((targetPosition - GetPositionRootSpace(bones[0])).sqrMagnitude >= completeLength * completeLength)
            {
                // just strech it
                Vector3 direction = (targetPosition - positions[0]).normalized;
                // set everything after root
                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
                }
            }
            else
            {
                for (int i = 0; i < positions.Length - 1; i++)
                {
                    positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + startDirections[i], snapBackStrength);
                }

                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    // https://www.youtube.com/watch?v=UNoX65PRehA
                    // back
                    for (int i = positions.Length - 1; i > 0; i--)
                    {
                        if (i == positions.Length - 1)
                        {
                            positions[i] = targetPosition; //set it to target
                        }
                        else
                        {
                            positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * boneLengths[i]; //set in line on distance
                        }
                    }

                    // forward
                    for (int i = 1; i < positions.Length; i++)
                    {
                        positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * boneLengths[i - 1];
                    }

                    // close enough?
                    if ((positions[positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                    {
                        break;
                    }
                }
            }

            // move towards pole
            Vector3 polePosition = GetPositionRootSpace(pole);
            for (int i = 1; i < positions.Length - 1; i++)
            {
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }

            // set position & rotation
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == positions.Length - 1)
                {
                    SetRotationRootSpace(bones[i], Quaternion.Inverse(targetRotation) * startTargetRotation * Quaternion.Inverse(startBoneRotations[i]));
                }
                else
                {
                    SetRotationRootSpace(bones[i], Quaternion.FromToRotation(startDirections[i], positions[i + 1] - positions[i]) * Quaternion.Inverse(startBoneRotations[i]));
                }
                SetPositionRootSpace(bones[i], positions[i]);
            }
        }

        Vector3 GetPositionRootSpace(Transform current)
        {
            return Quaternion.Inverse(root.rotation) * (current.position - root.position);
        }

        void SetPositionRootSpace(Transform current, Vector3 position)
        {
            current.position = root.rotation * position + root.position;
        }

        Quaternion GetRotationRootSpace(Transform current)
        {
            // inverse(after) * before => rot: before -> after            
            return Quaternion.Inverse(current.rotation) * root.rotation;
        }

        void SetRotationRootSpace(Transform current, Quaternion rotation)
        {
            current.rotation = root.rotation * rotation;
        }

        protected abstract void ResolveTargetRotation();
    }
}