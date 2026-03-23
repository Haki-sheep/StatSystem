using UnityEngine;

public class SimpleFc : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("视角设置")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookXLimit = 90f;

    [Header("组件")]
    [SerializeField] private Camera playerCamera;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private float _rotationX;

    private float _inputX;
    private float _inputZ;
    private bool _isSprinting;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
            _characterController = gameObject.AddComponent<CharacterController>();

        _characterController.minMoveDistance = 0;
        _characterController.slopeLimit = 45f;
    }

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleLookInput();
        ApplyMovement();
    }

    private void HandleMovementInput()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputZ = Input.GetAxisRaw("Vertical");
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleLookInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void ApplyMovement()
    {
        Vector3 move = transform.right * _inputX + transform.forward * _inputZ;

        float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;
        _characterController.Move(move * currentSpeed * Time.deltaTime);

        if (_characterController.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }
}
