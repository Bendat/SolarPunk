using System.IO.IsolatedStorage;
using UnityEngine;

namespace Player
{
    public class PlayerStateManager : Singleton<PlayerStateManager>
    {
        
        public Animator PlayerAnimator;

        public CharacterController PlayerCharacterController;

        public DetectInteractable DetectInteractable;

        public UIManager UIManager;

        public Transform WeaponTransform;

        public Camera MainCam;

        public bool IsMovementLocked = false;

        private IKHandler _handler;

        // Use this for initialization
        void Start()
        {
            PlayerAnimator = GetComponent<Animator>();
            PlayerCharacterController = GetComponent<CharacterController>();
            _handler = GetComponent<IKHandler>();
            MainCam = Camera.main;
            DetectInteractable = GetComponent<DetectInteractable>();
            UIManager = GetComponent<UIManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsMovementLocked)
            {
                AnimSetFloat("Speed", Mathf.Abs(PlayerCharacterController.velocity.z));
            }
            DirectWeaponToTarget();
        }

        public void OpenStorage(Storage storage)
        {
            
        }
        public void AnimSetFloat(string condition, float value)
        {
            PlayerAnimator.SetFloat(condition, value);
        }

        public void AnimSetBool(string condition, bool value)
        {
            PlayerAnimator.SetBool(condition, value);
        }

        private void  DirectWeaponToTarget()
        {
            //WeaponTransform.LookAt(_handler.EyeTarget);
        }

    }
}
