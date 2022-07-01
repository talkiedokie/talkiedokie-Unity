using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private CinemachineVirtualCamera _camView, _currentCamCache;
    [SerializeField] private FpCtrl_Input _fpsInput;

    [Header("General Configuration")]
    [SerializeField] private bool _isActivated;
    [SerializeField] private GameObject playerObject;

    [Header("Movement Configuration")]
    [SerializeField] private float _jumpForce = 1;
    [SerializeField] private float _movementSpeed = 1, _panningSpeed = 1;
    [SerializeField] private float _sprintMultiplier = 2;

    private Vector3 _movementAxis;
    private Vector2 _rotateAxis;
    private float _moveYVelocity;
    private float _gravity;
    private float _finalSpeed;

    [Header("Interaction Configuration")]
    [SerializeField] private LayerMask _clickablesLayer;
    [SerializeField] private float _clickableMinimumDistance;
    Clickable _hoveredObject;
    GameObject _interactionPromptUI;

    private Transform _camT;
    private Camera _cam;
    private CameraManager _camMgr;

    void Start()
    {

        _camView = GetComponentInChildren<CinemachineVirtualCamera>();
        _camT = _camView.transform;
        _cam = Camera.main;
        _camMgr = CameraManager.Instance;
        _characterController = GetComponent<CharacterController>();
        _gravity = Physics.gravity.y;
        // for debug
        EnableController();
    }

    private void EnableController()
    {
        // hook to fps input module
        _fpsInput = FpCtrl_Input.Instance;

        // activate UI
        _fpsInput.gameObject.SetActive(true);

        // hook interaction prompt
        _interactionPromptUI = _fpsInput.interactionPromptUI;

        // change camera to fps camera
        _camMgr?.SetPriority(_camView);

        _currentCamCache = _camMgr.Current;

        _isActivated = true;
        StartCoroutine(ActivatePlayerObject(false, 1));
    }

    public void OnEnable()
    {
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnDisable()
    {
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (playerObject != null)
            playerObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActivated)
            return;

        HandleJump();
        HandleRotation();
        HandleMovement();
        HandleHighlighting();
        HandleInteraction();
        MoveCharacter();

        if (Input.GetKeyDown(KeyCode.Tab))
            Application.targetFrameRate = Application.targetFrameRate == 60 ? -1 : 60;
    }

    private IEnumerator ActivatePlayerObject(bool isActive, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerObject != null)
            playerObject.SetActive(isActive);
    }

    private void HandleMovement()
    {
        // process data from fps input module
        _movementAxis = transform.TransformDirection(_fpsInput.moveAxis.x, 0, _fpsInput.moveAxis.y);
    }
    
    private void HandleRotation()
    {
        // process data from fps input module for left and right movement
        _rotateAxis = new Vector2(0, _fpsInput.panAxis.x);

        // rotate player body left and right
        transform.Rotate(_rotateAxis * _panningSpeed);

        //process data from fps input module for up and down movement
       _rotateAxis = new Vector2(_fpsInput.panAxis.y * -1, 0);

        // rotate camera up and down
        _camView.transform.Rotate(_rotateAxis * _panningSpeed);

        // clamp camera rotation
        var clampedX = _camView.transform.eulerAngles.x;
        if (_camView.transform.eulerAngles.x > 90 && _camView.transform.eulerAngles.x < 330)
            clampedX = 330f;

        if (_camView.transform.eulerAngles.x >= 0 && _camView.transform.eulerAngles.x < 90)
            clampedX = Mathf.Clamp(clampedX, 0, 30);

        _camView.transform.eulerAngles = new Vector3(clampedX, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void HandleJump()
    {
        // clamp y velocity to zero
        if (_characterController.isGrounded && _characterController.velocity.y < 0)
            _moveYVelocity = 0;

        // apply gravity
        _moveYVelocity += _gravity * Time.deltaTime;

        // Check Jump
        if (_fpsInput.jumpButtonDown && _characterController.isGrounded)
        {
            _moveYVelocity = _jumpForce;
        }
    }

    private void MoveCharacter()
    {
        _finalSpeed = _fpsInput.sprintButtonPressed ? _movementSpeed * _sprintMultiplier : _movementSpeed;

        _characterController.Move(
            new Vector3
            (
                _movementAxis.x * _finalSpeed, 
                _moveYVelocity, 
                _movementAxis.z * _finalSpeed
            )
            * Time.deltaTime
        );
    }

    private void HandleHighlighting()
    {
        var ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool raycast = Physics.Raycast(ray, out var hit, _clickableMinimumDistance, _clickablesLayer);

        if (raycast)
        {
            if (hit.collider.TryGetComponent<Clickable>(out var current))
            {
                _hoveredObject?.ShowHighlight(false);
                _hoveredObject = current;

                Highlight(true);
            }
            else 
                Highlight(false);
        }
        else 
            Highlight(false);
    }

    private void HandleInteraction()
    {
        //Debug.Log(_fpsInput.interactButtonClicked);
        if (_fpsInput.interactButtonClicked && _hoveredObject != null)
            _hoveredObject.Interact();


    }

    private void Highlight(bool isOn)
    {
        _hoveredObject?.ShowHighlight(isOn);

        if (_interactionPromptUI)
            _interactionPromptUI.SetActive(isOn);

        if (!isOn)
            _hoveredObject = null;
    }
}
