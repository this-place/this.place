using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject PlayerObject;

    //used for translation
    private Vector3 _offset;

    //used for rotation
    private bool _isKeyboardRotating;
    private bool _isMouseRotating;
    private const float KeyboardSpeed = 380f;
    private float _mouseX;
    private float _mouseY;
    private const float MouseXSpeed = 1;
    private const float MouseYSpeed = 0.1f;
    private float _currentYDisplacement;
    private const float MaxYDisplacement = 30;
    private PlayerController _playerController;


    private void Start()
    {
        _offset = transform.position - PlayerObject.transform.position;
        _playerController = PlayerObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        Vector3 newPosition = PlayerObject.transform.position + _offset;
        transform.position = newPosition;

        if (Input.GetAxis("CameraControl") != 0 && !_isMouseRotating)
        {
            _isKeyboardRotating = true;
        }

        if (Input.GetAxis("CameraControl") == 0 && !_isMouseRotating)
        {
            _isKeyboardRotating = false;
        }

        if (Input.GetMouseButtonDown(1) && !_isKeyboardRotating)
        {
            _isMouseRotating = true;
            _mouseX = Input.mousePosition.x;
            _mouseY = Input.mousePosition.y;
        }

        if (Input.GetMouseButtonUp(1) && !_isKeyboardRotating)
            _isMouseRotating = false;

        if (_isKeyboardRotating)
            RotateKeyboardCamera();

        if (_isMouseRotating)
        {
            RotateMouseCamera();
        }
    }

    private void RotateKeyboardCamera()
    {
        float moveAmount = Input.GetAxis("CameraControl") * Time.deltaTime * KeyboardSpeed;
        transform.RotateAround(PlayerObject.transform.position, Vector3.up, moveAmount);

        _playerController.UpdateCamera();
        _offset = transform.position - PlayerObject.transform.position;
    }

    private void RotateMouseCamera()
    {
        float rotateXAmount = (_mouseX - Input.mousePosition.x) * MouseXSpeed;
        float rotateYAmount = (_mouseY - Input.mousePosition.y) * MouseYSpeed;
        _currentYDisplacement += rotateYAmount;
        if (_currentYDisplacement > MaxYDisplacement)
        {
            rotateYAmount -= _currentYDisplacement - MaxYDisplacement;
            _currentYDisplacement = MaxYDisplacement;
        }
        else if (_currentYDisplacement < -MaxYDisplacement)
        {
            rotateYAmount -= _currentYDisplacement + MaxYDisplacement;
            _currentYDisplacement = -MaxYDisplacement;
        }
        transform.RotateAround(PlayerObject.transform.position, Vector3.up, rotateXAmount);
        Vector3 normalVector = GetNormalVector();
        transform.RotateAround(PlayerObject.transform.position, normalVector, rotateYAmount);
        _mouseX = Input.mousePosition.x;
        _mouseY = Input.mousePosition.y;

        _playerController.UpdateCamera();
        _offset = transform.position - PlayerObject.transform.position;
    }

    private Vector3 GetNormalVector()
    {
        Vector3 playerFacing = transform.position - PlayerObject.transform.position;
        Vector3 planeCompletion = playerFacing - Vector3.up;
        return Vector3.Cross(playerFacing, planeCompletion);
    }
}
