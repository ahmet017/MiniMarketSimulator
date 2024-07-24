using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController Instance;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 5.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 8.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private PlayerController _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;   
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerController>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            Pickup();
            MoveObject();

        }


        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }

        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        [Header("pickupTools")]
        [SerializeField] private float pickupRange;
        [SerializeField] private LayerMask InteractableObj, Shelf, boxlayer, currentitem;
        [SerializeField] private Transform itemHolder;
        public Rigidbody itemRb;
        private Collider itemCollider;
        bool isOpen;
        public Animator Animator;

        public void Pickup()
        {
            
            if(itemRb != null)
            {
                Animator.SetBool("IsOpen", isOpen);
                TakeText.SetActive(false);
            }

            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, InteractableObj))
            {
                TakeText.SetActive(itemRb == null);

                if (itemRb == null && _input.Pickup)
                {
                    HoldItem(hit);
                    _input.Pickup = false;
                }

                
            }

            if(!Physics.Raycast(ray, out hit, pickupRange, InteractableObj))
            {
                TakeText.SetActive(false);
            }
            if (itemRb != null && _input.Open)
            {
                isOpen = !isOpen;
                _input.Open = false;
            }

            else if (Physics.Raycast(ray, out hit, pickupRange, Shelf)) // Raycast for shelf
            {
                if (itemRb != null && _input.Click) // Place on shelf if holding item
                {
                    Transform itemChild = null;
                    ShelfManager shelfManager = hit.collider.GetComponent<ShelfManager>();
                    if (itemRb.tag == "box")
                    {
                        if(shelfManager != null)
                        {
                            foreach (Transform child in itemRb.transform)
                            {
                                
                                if (child.tag == "Item")
                                {
                                    itemChild = child;
                                }

                            }
                            if (itemChild != null)
                            {
                                shelfManager.PlaceItemOnShelf(itemChild.gameObject);
                                BoxManager.instance.UrunKaldir();
                            }
                        }
                    }
                    else if(shelfManager != null && itemRb.tag != "box")
                    {
                        shelfManager.PlaceItemOnShelf(itemRb.transform.gameObject);
                    }

                    _input.Click = false;
                }

                if (itemRb != null && _input.RightClick) // Place on shelf if holding item
                {
                    ShelfManager shelfManager = hit.collider.GetComponent<ShelfManager>();
                    if (itemRb.tag == "box")
                    {
                        Transform lastChild = hit.collider.gameObject.transform.GetChild(hit.collider.gameObject.transform.childCount - 1);
                        Debug.Log(lastChild);
                        Debug.Log(itemRb);
                        Debug.Log(BoxManager.instance.urunYerleri[BoxManager.instance.yerIndex].transform.localPosition);
                        BoxManager.instance.UrunEkle();
                        PickUpItemFromShelfToBox(lastChild, itemRb.transform, BoxManager.instance.urunYerleri[BoxManager.instance.yerIndex].transform.localPosition); // Refactored for clarity
                    }
                    else if (shelfManager != null && itemRb.tag != "box")
                    {
                        shelfManager.PlaceItemOnShelf(itemRb.transform.gameObject);
                    }

                    _input.RightClick = false;
                }
                else if (_input.Click) // Pick up from shelf if no item held
                {
                    if (hit.collider.gameObject.transform.childCount > 0)
                    {
                        Transform lastChild = hit.collider.gameObject.transform.GetChild(hit.collider.gameObject.transform.childCount - 1);
                        PickUpItemFromShelf(lastChild, _mainCamera.transform, itemHolder.localPosition); // Refactored for clarity
                        
                    }
                    
                    _input.Click = false;
                }
            }
            else // No hit or wrong layer
            {
                if (_input.Pickup)
                {
                    itemRb.gameObject.transform.parent = null;
                    itemRb = null;
                    itemCollider = null;
                    _input.Pickup = false;

                }// Reset pickup state if button pressed
                    

            }
        }
        private void PickUpItemFromShelfToBox(Transform child, Transform parent, Vector3 endPos)
        {
            //itemRb = child.GetComponent<Rigidbody>();
            //itemCollider = child.GetComponent<Collider>();


            child.transform.parent = null;
            child.gameObject.transform.parent = parent.transform;
            child.gameObject.layer = 7;

            child.transform.localRotation = itemHolder.transform.localRotation;
            child.transform.localPosition = endPos;
            //Vector3 startPos = itemRb.transform.localPosition;
            //StartCoroutine(MoveTowardsSmooth(startPos, endPos));
            if (itemRb != null)
            {
                itemRb.isKinematic = true;
            }
        }
        private void PickUpItemFromShelf(Transform child, Transform parent, Vector3 endPos)
        {
            //itemRb = child.GetComponent<Rigidbody>();
            //itemCollider = child.GetComponent<Collider>();


            child.transform.parent = null;
            child.gameObject.transform.parent = parent.transform;
            child.gameObject.layer = 7;

            child.transform.localRotation = itemHolder.transform.localRotation;
            Vector3 startPos = itemRb.transform.localPosition;
            StartCoroutine(MoveTowardsSmooth(startPos, endPos));
            if (itemRb != null)
            {
                itemRb.isKinematic = true;
            }
        }
        public void HoldItem(RaycastHit hit)
        {
            itemRb = hit.rigidbody;
            itemCollider = hit.collider;

            if (hit.rigidbody != null)
            {
                itemRb.isKinematic = true;
            }

            itemCollider.gameObject.transform.parent = _mainCamera.transform;
            itemCollider.gameObject.layer = 7;

            foreach (Transform child in itemCollider.transform)
            {
                child.gameObject.layer = 7;
            }

            itemRb.transform.localRotation = itemHolder.transform.localRotation;
            Vector3 startPos = itemRb.transform.localPosition;
            Vector3 endPos = itemHolder.localPosition;
            StartCoroutine(MoveTowardsSmooth(startPos, endPos));
        }
        IEnumerator MoveTowardsSmooth(Vector3 startPos, Vector3 targetPos)
        {
            float speed = 15.0f; // Adjust for desired speed

            while (Vector3.Distance(itemRb.transform.localPosition, targetPos) > 0.01f)
            {
                itemRb.transform.localPosition = Vector3.MoveTowards(itemRb.transform.localPosition, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            // Ensure final position is reached
            itemRb.transform.localPosition = targetPos;
        }

        public void DropItem(LayerMask layer)
        {
            itemCollider.gameObject.layer = layer;
            itemRb.isKinematic = false;
            itemCollider = null;
            itemRb = null;

        }

        [Header("MovableObjTools")]
        public Transform MovableObj;
        public Collider MovableCollider;
        public LayerMask MovableObjLayer;
        public float ObjRotationSpeed = 500;

        public Material GhostMaterial;
        public Material MovableMaterial;

        [Header("TextsOnScreen")]
        public GameObject TakeText;
        public GameObject MoveText;
        public GameObject RotateAndConfirmText;

        public void MoveObject()
        {
            ObjectPlacement();
            RotateObject();
        }
        Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        public void ObjectPlacement()
        {
            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, MovableObjLayer))
            {
                if (MovableObj == null)
                {
                    MoveText.SetActive(true);
                    RotateAndConfirmText.SetActive(false);

                    if (_input.MoveObj)
                    {
                        Renderer[] childRenderers = hit.transform.GetComponentsInChildren<Renderer>();
                        MovableObj = hit.transform.GetComponentInParent<Transform>();
                        foreach (Renderer renderer in childRenderers)
                        {
                            originalMaterials.Add(renderer, renderer.material);
                            renderer.material = GhostMaterial;
                        }

                        foreach (Collider childCollider in MovableObj.GetComponentsInChildren<Collider>())
                        {
                            childCollider.enabled = false;
                        }

                    }



                }

                else
                {
                    MoveText.SetActive(false);
                    RotateAndConfirmText.SetActive(true);

                }
            }

            else
            {
                MoveText.SetActive(false);
                if(MovableObj != null)
                    RotateAndConfirmText.SetActive(true);
                else
                {
                    RotateAndConfirmText.SetActive(false);

                }

            }

            if (MovableObj != null)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (MovableObj != null)
                    {
                        MovableObj.transform.position = hit.point;
                    }

                }

            }

            if (_input.Click && MovableObj != null)
            {
                foreach (KeyValuePair<Renderer, Material> pair in originalMaterials)
                {
                    pair.Key.material = pair.Value;
                }
                foreach (Collider childCollider in MovableObj.GetComponentsInChildren<Collider>())
                {
                    childCollider.enabled = true;
                }
                originalMaterials.Clear();
                MovableObj = null;
                Debug.Log("tiklamdi");
            }
        }
        private void RotateObject()
        {
            if (MovableObj != null)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    MovableObj.Rotate(Vector3.up * ObjRotationSpeed * Time.deltaTime);

                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    MovableObj.Rotate(Vector3.up * -ObjRotationSpeed * Time.deltaTime);

                }

            }


        }
    }
}