using UnityEngine;

namespace ProceduralAnimation.IK
{
    class PlayerIKManager : MonoBehaviour
    {
        public GameObject rightFoot;
        public GameObject leftFoot;
        public GameObject rightHand;
        public GameObject leftHand;

        SpineIK spineIK;
        FootIK rightFootIK;
        FootIK leftFootIK;
        HandIK rightHandIK;
        HandIK leftHandIK;

        void Start()
        {
            spineIK = SpineIK.CreateComponent(gameObject, "Spine IK", transform.parent.parent);
            // rightFootIK = FootIK.CreateComponent(rightFoot, "Right Foot IK", spineIK.target, new Vector3(0, 0, -1));
            // leftFootIK = FootIK.CreateComponent(leftFoot, "Left Foot IK", spineIK.target, new Vector3(0, 0, 1));
            rightHandIK = HandIK.CreateComponent(rightHand, "Right Hand IK", transform.parent.parent, new Vector3(.5f, 0, -.5f), new Vector3(.5f, 0, -.5f));
            leftHandIK = HandIK.CreateComponent(leftHand, "Left Hand IK", transform.parent.parent, new Vector3(-.5f, 0, .5f), new Vector3(-.5f, 0, .5f));
        }
    }
}