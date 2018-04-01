using System.Collections;
using UnityEngine;

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
    private const float KeyboardSpeed = 380f;
    private const float MouseXSpeed = 3f;
    private const float MouseYSpeed = 1f;
    private float _currentYDisplacement;
    private const float MaxYDisplacement = 50f;
    private PlayerController _playerController;
    private ArrayList _fadeBlocks = new ArrayList();

    //used for zoom
    private const float MaxZoom = 6;
    private const float MinZoom = -6;
    private const float zoomSpeed = 3;
    private float currentZoom = 0;

    private void Start()
    {
        _offset = new Vector3(StartingXOffset, StartingYOffset, StartingZOffset);
        transform.eulerAngles = new Vector3(StartingXRotation, StartingYRotation, StartingZRotation);
        _playerController = PlayerObject.GetComponent<PlayerController>();
        _playerController.UpdateCamera();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 newPosition = PlayerObject.transform.position + _offset;
        transform.position = newPosition;
        if (Input.GetAxis("CameraControl") != 0)
        {
            _isKeyboardRotating = true;
        }

        if (Input.GetAxis("CameraControl") == 0)
        {
            _isKeyboardRotating = false;
        }

        if (_isKeyboardRotating)
        {
            RotateKeyboardCamera();
        }

        RotateMouseCamera();

        if (Input.GetAxis("CameraZoom") > 0 || Input.GetAxis("CameraZoom") < 0)
        {
            ZoomCamera(Input.GetAxis("CameraZoom"));
        }

        UpdateFadingBlocks();
    }

    //returns true if zoom happened, false if not
    private bool ZoomCamera(float zoomAmount)
    {
        float zoomDifference = zoomSpeed * zoomAmount;
        if (!(currentZoom + zoomDifference > MaxZoom) && !(currentZoom + zoomDifference < MinZoom))
        {
            _offset = transform.position - PlayerObject.transform.position + Vector3.Normalize(transform.position - PlayerObject.transform.position) * zoomDifference;
            currentZoom += zoomDifference;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateFadingBlocks()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 cameraToPlayerPosition = cameraPosition - PlayerObject.transform.position;
        cameraToPlayerPosition.y = 0;
        foreach (FadeoutPlugin fadeoutPlugin in _fadeBlocks)
        {
            if (fadeoutPlugin.GetBlock() == null) continue;
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
        float rotateXAmount = (Input.GetAxisRaw("Mouse X")) * MouseXSpeed;
        float rotateYAmount = (Input.GetAxisRaw("Mouse Y")) * MouseYSpeed;
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
