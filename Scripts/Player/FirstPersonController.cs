using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        public Transform FPSCamera;
        #region MovementPublic

        public float WalkSpeed = 6.0f;

        public float RunSpeed = 11.0f;

        // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
        public bool LimitDiagonalSpeed = true;

        // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
        // There must be a button set up in the Input Manager called "Run"
        public bool ToggleRun = false;

        public float JumpSpeed = 8.0f;

        public float Gravity = 20.0f;

        // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
        public float FallingDamageThreshold = 10.0f;

        // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
        public bool SlideWhenOverSlopeLimit = false;

        // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
        public bool SlideOnTaggedObjects = false;

        public float SlideSpeed = 12.0f;

        // If checked, then the player can change direction while in the air
        public bool AirControl = false;

        // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
        public float AntiBumpFactor = .75f;

        // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
        public int AntiBunnyHopFactor = 1;

        #endregion MovementPublic

        #region MousePublic

        public enum RotationAxes
        {
            MouseXAndY = 0,
            MouseX = 1,
            MouseY = 2
        }

        public RotationAxes axes = RotationAxes.MouseXAndY;

        public float SensitivityX = 15F;
        public float SensitivityY = 15F;

        public float MinimumX = -360F;
        public float MaximumX = 360F;

        public float MinimumY = -60F;
        public float MaximumY = 60F;

        #endregion MousePublic

        #region MovementPrivate

        private Vector3 _moveDirection = Vector3.zero;
        private bool _grounded = false;
        private CharacterController _controller;
        private Transform _myTransform;
        private float _speed;
        private RaycastHit _hit;
        private float _fallStartLevel;
        private bool _falling;
        private float _slideLimit;
        private float _rayDistance;
        private Vector3 _contactPoint;
        private bool _playerControl = false;
        private int _jumpTimer;

        #endregion

        #region MousePrivate
        private float _rotAverageY = 0F;
        private float _rotationX = 0F;
        private float _rotationY = 0F;
        private Quaternion _originalRotationCamera;
        private Quaternion _originalRotationBody;

        #endregion MousePrivate

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _myTransform = transform;
            _speed = WalkSpeed;
            _rayDistance = _controller.height * .5f + _controller.radius;
            _slideLimit = _controller.slopeLimit - .1f;
            _jumpTimer = AntiBunnyHopFactor;
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
            if (!FPSCamera)
            {
                FPSCamera = GetComponentsInChildren<Camera>()[0].transform;
            }
            _originalRotationCamera = FPSCamera.transform.localRotation;
            _originalRotationBody = transform.localRotation;

        }

        void FixedUpdate()
        {
            MoveRaw();
        }

        void Update()
        {
            // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
            // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
            if (ToggleRun && _grounded && Input.GetButtonDown("Run"))
                _speed = (Math.Abs(_speed - WalkSpeed) < 0.1f ? RunSpeed : WalkSpeed);
            Look();
        }
        
        private void MoveRaw()
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && LimitDiagonalSpeed) ? .7071f : 1.0f;

            if (_grounded)
            {
                bool sliding = false;
                // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
                // because that interferes with step climbing amongst other annoyances
                if (Physics.Raycast(_myTransform.position, -Vector3.up, out _hit, _rayDistance))
                {
                    if (Vector3.Angle(_hit.normal, Vector3.up) > _slideLimit)
                        sliding = true;
                }
                // However, just raycasting straight down from the center can fail when on steep slopes
                // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
                else
                {
                    Physics.Raycast(_contactPoint + Vector3.up, -Vector3.up, out _hit);
                    if (Vector3.Angle(_hit.normal, Vector3.up) > _slideLimit)
                        sliding = true;
                }

                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (_falling)
                {
                    _falling = false;
                    if (_myTransform.position.y < _fallStartLevel - FallingDamageThreshold)
                        FallingDamageAlert(_fallStartLevel - _myTransform.position.y);
                }

                // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
                if (!ToggleRun)
                    _speed = Input.GetButton("Run") ? RunSpeed : WalkSpeed;

                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ((sliding && SlideWhenOverSlopeLimit) || (SlideOnTaggedObjects && _hit.collider.CompareTag("Slide")))
                {
                    Vector3 hitNormal = _hit.normal;
                    _moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref _moveDirection);
                    _moveDirection *= SlideSpeed;
                    _playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else
                {
                    _moveDirection = new Vector3(inputX * inputModifyFactor, -AntiBumpFactor,
                        inputY * inputModifyFactor);
                    _moveDirection = _myTransform.TransformDirection(_moveDirection) * _speed;
                    _playerControl = true;
                }

                // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                if (!Input.GetButton("Jump"))
                    _jumpTimer++;
                else if (_jumpTimer >= AntiBunnyHopFactor)
                {
                    _moveDirection.y = JumpSpeed;
                    _jumpTimer = 0;
                }
            }
            else
            {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!_falling)
                {
                    _falling = true;
                    _fallStartLevel = _myTransform.position.y;
                }

                // If air control is allowed, check movement but don't touch the y component
                if (AirControl && _playerControl)
                {
                    _moveDirection.x = inputX * _speed * inputModifyFactor;
                    _moveDirection.z = inputY * _speed * inputModifyFactor;
                    _moveDirection = _myTransform.TransformDirection(_moveDirection);
                }
            }

            // Apply gravity
            _moveDirection.y -= Gravity * Time.deltaTime;

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            _grounded = (_controller.Move(_moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
        private void Look()
        {
            switch (axes)
            {
                case RotationAxes.MouseXAndY: {
                    // Read the mouse input axis
                    _rotationX += Input.GetAxis("Mouse X") * SensitivityX;
                    _rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
                    _rotationX = ClampAngle(_rotationX, MinimumX, MaximumX);
                    _rotationY = ClampAngle(_rotationY, MinimumY, MaximumY);
                    Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
                    Quaternion yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);

                    FPSCamera.transform.localRotation = _originalRotationCamera * yQuaternion;
                    transform.localRotation = _originalRotationBody * xQuaternion;
                }
                    break;
                case RotationAxes.MouseX: {
                    _rotationX += Input.GetAxis("Mouse X") * SensitivityX;
                    _rotationX = ClampAngle(_rotationX, MinimumX, MaximumX);
                    Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
                    transform.localRotation = _originalRotationCamera * xQuaternion;
                }
                    break;
                default: {
                    _rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
                    _rotationY = ClampAngle(_rotationY, MinimumY, MaximumY);
                    Quaternion yQuaternion = Quaternion.AngleAxis(-_rotationY, Vector3.right);
                    transform.localRotation = _originalRotationCamera * yQuaternion;
                }
                    break;
            }
        }

        private float ClampAngle(float angle, float minimumY, float maximumY)
        {
//            if (angle < -360F)
//                angle += 360F;
//            if (angle < 360F)
//                angle -= 360F;
            return Mathf.Clamp(angle%360, minimumY, maximumY);
        }

        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _contactPoint = hit.point;
        }

        // If falling damage occured, this is the place to do something about it. You can make the player
        // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
        void FallingDamageAlert(float fallDistance)
        {
            print("Ouch! Fell " + fallDistance + " units!");
        }

    }
}
