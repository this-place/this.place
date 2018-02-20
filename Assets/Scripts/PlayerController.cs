using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private const float MoveSpeed = 2f;
    private const float JumpForce = 5.2f;

    private Vector3 _forward;
    private Vector3 _right;

    private bool _onGround = true;

    private Rigidbody _rb;

    // Use this for initialization
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _forward = Camera.main.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Move();
        }

        if (_onGround && Input.GetAxisRaw("Jump") == 1.0f)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(0, Input.GetAxis("Jump") * JumpForce, 0);
        _onGround = false;
    }

    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = _right * MoveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = _forward * MoveSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    private void OnCollisionStay()
    {
        _onGround = true;
    }
}
