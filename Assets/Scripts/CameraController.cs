using System;
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public GameObject PlayerObject;

    //used for translation
    private Vector3 _offset;
    private float _yValue;

    //used for rotation
    private float _originalY;
    private float _targetY;
    private bool _isKeyboardRotating;
    private bool _isMouseRotating;
    private float _keyboardSpeed = 380f;
    private float _movedAmount;
    private float _mouseX;
    private float _mouseY;
    private float _mouseXSpeed = 1;
    private float _mouseYSpeed = 0.1f;
    private float _currentYDisplacement;
    private const float MaxYDisplacement = 30;


    private void Start()
    {
        _offset = transform.position - PlayerObject.transform.position;
        //_offset.y = 0;
        _yValue = transform.position.y;
        _originalY = transform.position.y;
    }

    private void Update()
    {
        Vector3 newPosition = PlayerObject.transform.position + _offset;
        //newPosition.y = _yValue;
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
        float moveAmount = Input.GetAxis("CameraControl") * Time.deltaTime * _keyboardSpeed;
        transform.RotateAround(PlayerObject.transform.position, Vector3.up, moveAmount);

        PlayerObject.GetComponent<PlayerController>().UpdateCamera();
        _offset = transform.position - PlayerObject.transform.position;
        //_offset.y = 0;
    }

    private void RotateMouseCamera()
    {
        float rotateXAmount = (_mouseX - Input.mousePosition.x) * _mouseXSpeed;
        float rotateYAmount = (_mouseY - Input.mousePosition.y) * _mouseYSpeed;
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

        PlayerObject.GetComponent<PlayerController>().UpdateCamera();
        _offset = transform.position - PlayerObject.transform.position;
        //_offset.y = 0;
    }

    private Vector3 GetNormalVector()
    {
        Vector3 playerFacing = transform.position - PlayerObject.transform.position;
        Vector3 planeCompletion = playerFacing - Vector3.up;
        return Vector3.Cross(playerFacing, planeCompletion);
    }

    private void LateUpdate()
    {
    }
}
