using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public GameObject PlayerObject;

    //used for translation
    private Vector3 _offset;
    private float _yValue;
    public float Speed = 0.25f;

    //used for rotation
    private float _originalY;
    private float _targetY;
    private bool _isRotating;

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
            _targetY = transform.eulerAngles.y + Input.GetAxis("CameraControl") * 90;
            _isRotating = true;
        }

        if (_isRotating)
        {
            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        float rotateAmount = _targetY - _originalY;
        transform.RotateAround(PlayerObject.transform.position, Vector3.up, rotateAmount * Time.deltaTime * Speed);
        float remainingRotation = Mathf.Abs(_targetY - transform.eulerAngles.y);
        /*if (remainingRotation >= 270)
            remainingRotation -= 269.9f;
        if (remainingRotation <= -270)
            remainingRotation += 269.9f;*/
        if (remainingRotation <= 0.1f)
        {
            //transform.rotation = new Quaternion(transform.rotation.x, _targetY, transform.rotation.z, transform.rotation.w);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _targetY, transform.eulerAngles.z);
            _originalY = _targetY;
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
