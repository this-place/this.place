using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{

    public GameObject PlayerObject;
    public const float StartingXOffset = -4;
    public const float StartingYOffset = 4;
    public const float StartingZOffset = -4;
    public const float StartingXRotation = 30;
    public const float StartingYRotation = 45;
    public const float StartingZRotation = 0;

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
    private ArrayList _fadeBlocks = new ArrayList();

    private void Start()
    {
        _offset = new Vector3(StartingXOffset, StartingYOffset, StartingZOffset);
        transform.eulerAngles = new Vector3(StartingXRotation, StartingYRotation, StartingZRotation);
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
        {
            RotateKeyboardCamera();
        }

        if (_isMouseRotating)
        {
            RotateMouseCamera();
        }

 
        UpdateFadingBlocks();

    }

    private void UpdateFadingBlocks()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 cameraToPlayerPosition = cameraPosition - PlayerObject.transform.position;
        cameraToPlayerPosition.y = 0;
        foreach (FadeoutPlugin fadeoutPlugin in _fadeBlocks)
        {
            Vector3 cameraToBlockPosition =
                cameraPosition - fadeoutPlugin.GetBlock().transform.position;
            cameraToBlockPosition.y = 0;
            fadeoutPlugin.SetIsFading(cameraToBlockPosition.magnitude < cameraToPlayerPosition.magnitude);

        }
    }

    public void AddFadeoutTarget(FadeoutPlugin fadeoutPlugin)
    {
        _fadeBlocks.Add(fadeoutPlugin);
    }

    public void RemoveFadeoutTarget(FadeoutPlugin fadeoutPlugin)
    {
        _fadeBlocks.Remove(fadeoutPlugin);
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
