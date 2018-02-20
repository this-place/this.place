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
    private bool _isRotating;
    private float _speed = 380f;
    private float _movedAmount;

    private void Start()
    {
        _offset = transform.position - PlayerObject.transform.position;
        _offset.y = 0;
        _yValue = transform.position.y;
        _originalY = transform.position.y;
    }

    private void Update()
    {
        if ((Input.GetAxis("CameraControl") == 1 || Input.GetAxis("CameraControl") == -1) && !_isRotating)
        {
            _originalY = transform.eulerAngles.y;
            _targetY = transform.eulerAngles.y + Input.GetAxis("CameraControl") * 90;
            _isRotating = true;
            _movedAmount = 0;
        }

        if (_isRotating)
        {
            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        float rotateDir = (_targetY - _originalY)/90;
        float moveAmount = rotateDir * Time.deltaTime * _speed;
        _movedAmount += moveAmount;
        if (_movedAmount > 90)
            moveAmount -= _movedAmount - 90;
        else if (_movedAmount < -90)
            moveAmount -= _movedAmount + 90;
        transform.RotateAround(PlayerObject.transform.position, Vector3.up, moveAmount);
        if (Mathf.Abs(_movedAmount) >= 90f)
        {
            _isRotating = false;
        }

        PlayerObject.GetComponent<PlayerController>().UpdateCamera();
        _offset = transform.position - PlayerObject.transform.position;
        _offset.y = 0;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = PlayerObject.transform.position + _offset;
        newPosition.y = _yValue;
        transform.position = newPosition;
    }
}
