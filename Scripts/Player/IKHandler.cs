using UnityEngine;

namespace Player
{
    [ExecuteInEditMode]
    public class IKHandler : MonoBehaviour
    {
        public Transform EyeTarget;

        public Transform LeftHandTarget;
        public Transform RightHandTarget;

        public Transform RightHand;
        public Transform LeftHand;

        public Transform HeadTracker;

        public float LookIkWeight;

        public float BodyWeight;

        public float HeadWeight;

        public float EyesWeight;

        public float ClampWeight;

        private Vector3 _lastRightHandPosition;

        private Vector3 _lastLeftHandPosition;

        // Use this for initialization
        void Start()
        {
            _lastLeftHandPosition = LeftHand.transform.position;
            _lastRightHandPosition = RightHand.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.position*15);

            EyeTarget.position = ray.GetPoint(15);

        }

        void OnAnimatorIK(int layerIndex)
        {
//            Debug.Log("Called Ik");
            var anim = PlayerStateManager.Instance.PlayerAnimator;
            anim.SetLookAtWeight(LookIkWeight, BodyWeight, HeadWeight, EyesWeight, ClampWeight);
            anim.SetLookAtPosition(EyeTarget.position);

            if (LeftHandTarget != null && RightHandTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            }
        }

        void LateUpdate()
        {
            Camera.main.transform.position = HeadTracker.position - new Vector3(0, 0, 0.05f);
        }
    }
}
