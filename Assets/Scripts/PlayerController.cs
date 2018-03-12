using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask Layer;

    [SerializeField]
    private const float MoveSpeed = 2f;
    private const float JumpForce = 5.2f;
    private const float DistToGround = 0.5f;

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _heading;

    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private PlayerAnimatorController _animator;
    private Vector3[] _groundSkinVertices = new Vector3[40];
    private Vector3[] _forwardSkinVertices = new Vector3[16];
    private bool _isMobile = true;

    private const float GroundSkinOffset = 0.5f;
    private const float ForwardSkinOffset = 0.1f;

    private List<BlockBehaviour> _blockList = new List<BlockBehaviour>();

    // Use this for initialization
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<PlayerAnimatorController>();
        _boxCollider = this.gameObject.GetComponent<BoxCollider>();
        _heading = Vector3.forward;
        float groundXBound = _boxCollider.bounds.extents.x - GroundSkinOffset;
        float groundZBound = _boxCollider.bounds.extents.z - GroundSkinOffset;
        _groundSkinVertices[0] = new Vector3(groundXBound, 0, groundZBound);
        _groundSkinVertices[1] = new Vector3(groundXBound, 0, -groundZBound);
        _groundSkinVertices[2] = new Vector3(-groundXBound, 0, -groundZBound);
        _groundSkinVertices[3] = new Vector3(-groundXBound, 0, groundZBound);
        for (int i = 0; i < 4; i++)
        {
            float xDiff = _groundSkinVertices[(i + 1) % 3].x - _groundSkinVertices[i].x;
            float zDiff = _groundSkinVertices[(i + 1) % 3].z - _groundSkinVertices[i].z;
            float zStart = _groundSkinVertices[i].z;
            float xStart = _groundSkinVertices[i].x;

            for (int j = 0; j < 9; j++)
            {
                _groundSkinVertices[j + 4 + (9 * i)] = new Vector3(xStart + ((xDiff / 9) * (j + 1)),
                                                                   0,
                                                                   zStart + ((zDiff / 9) * (j + 1)));
            }
        }

        float forwardYBound = _boxCollider.bounds.extents.y - ForwardSkinOffset;
        float forwardZBound = _boxCollider.bounds.extents.z - ForwardSkinOffset;
        _forwardSkinVertices[0] = new Vector3(0, forwardYBound, forwardZBound);
        _forwardSkinVertices[1] = new Vector3(0, forwardYBound, -forwardZBound);
        _forwardSkinVertices[2] = new Vector3(0, -forwardYBound, -forwardZBound);
        _forwardSkinVertices[3] = new Vector3(0, -forwardYBound, forwardZBound);
        for (int i = 0; i < 4; i++)
        {
            float yDiff = _forwardSkinVertices[(i + 1) % 3].y - _forwardSkinVertices[i].y;
            float zDiff = _forwardSkinVertices[(i + 1) % 3].z - _forwardSkinVertices[i].z;
            float yStart = _forwardSkinVertices[i].y;
            float zStart = _forwardSkinVertices[i].z;

            for (int j = 0; j < 3; j++)
            {
                _forwardSkinVertices[j + 4 + (3 * i)] = new Vector3(0,
                                                                    yStart + ((yDiff / 4) * (j + 1)),
                                                                    zStart + ((zDiff / 4) * (j + 1)));
            }
        }
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        _forward = Camera.main.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
    }

    // Update is called once per frame
    private void Update()
    {

        if ((Mathf.Abs(Input.GetAxis("Horizontal")) != 0 || Mathf.Abs(Input.GetAxis("Vertical")) != 0) && _isMobile)
        {
            _animator.MovePlayer();
            Move();
        }
        else
        {
            _animator.StopPlayer();
        }

        if (Input.GetAxis("Jump") == 1.0f && IsOnGround() && _isMobile)
        {
            Jump();
        }

        // Checking for blocks the player is standing on
        foreach (BlockBehaviour block in _blockList)
        {
            block.SetIsPlayerStandingOn(false);
        }

        _blockList = new List<BlockBehaviour>();

        for (int i = 0; i < 40; i++)
        {

            RaycastHit hit;
            Debug.DrawRay(_boxCollider.bounds.center + _groundSkinVertices[i], Vector3.down);
            if (Physics.Raycast(_boxCollider.bounds.center + _groundSkinVertices[i], Vector3.down, out hit))
            {
                BlockBehaviour hitBlock = hit.collider.GetComponent<BlockBehaviour>();
                BlockFaceBehaviour hitBlockFace = hit.collider.GetComponent<BlockFaceBehaviour>();
                if (hitBlock != null && hit.normal == Vector3.up && !hitBlockFace.FireRaycastFromFace(0.1f, Layer, BlockFace.Top))
                {
                    hitBlock.SetIsPlayerStandingOn(true);

                    if (!_blockList.Contains(hitBlock))
                    {
                        _blockList.Add(hitBlock);
                    }
                }
            }
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(0, Input.GetAxis("Jump") * JumpForce, 0);
    }

    private void Move()
    {
        Vector3 rightMovement = _right * MoveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = _forward * MoveSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        _heading = Vector3.Normalize(rightMovement + upMovement);

        if (_heading != Vector3.zero)
            transform.forward = _heading;

        Vector3 movement = CheckCollision(rightMovement + upMovement);

        transform.position += movement;
    }

    private Vector3 CheckCollision(Vector3 movement)
    {
        float closestPoint = float.MaxValue;
        Vector3 correctNormal = Vector3.zero;
        bool hit = false;
        float angle = Mathf.Atan2(transform.forward.z, transform.forward.x);
        foreach (Vector3 skinVertex in _forwardSkinVertices)
        {
            float newXValue = Mathf.Cos(angle) * skinVertex.x - Mathf.Sin(angle) * skinVertex.z;
            float newZValue = Mathf.Sin(angle) * skinVertex.x + Mathf.Cos(angle) * skinVertex.z;
            Debug.DrawRay(_boxCollider.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue), transform.forward * 2f, Color.red);
            RaycastHit[] rayHits = Physics.RaycastAll(_boxCollider.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue),
                transform.forward, movement.magnitude + _boxCollider.bounds.extents.x);
            if (rayHits.Length != 0)
            {
                if (Physics.Raycast(_boxCollider.bounds.center, transform.forward, 2f))
                    correctNormal = Physics.RaycastAll(_boxCollider.bounds.center, transform.forward, 2f)[0].normal;
                else
                    correctNormal = rayHits[0].normal;

                foreach (RaycastHit rayHit in rayHits)
                {
                    float distance = rayHit.distance;
                    if (distance < closestPoint)
                    {
                        closestPoint = distance;
                        hit = true;
                    }
                }
            }
        }

        if (hit)
        {
            if (correctNormal != -transform.forward && (closestPoint - _boxCollider.bounds.extents.x) < 0.01 && IsOnGround())
            {
                transform.forward = Vector3.Normalize(transform.forward - Vector3.Project(transform.forward, correctNormal));
                return CheckCollision(movement);
            }
            return (closestPoint - _boxCollider.bounds.extents.x) * Vector3.Normalize(movement) * 0.95f;
        }
        else
        {
            return movement.magnitude * transform.forward;
        }
    }

    private bool IsOnGround()
    {
        float angle = Mathf.Atan2(_heading.x, _heading.z);
        foreach (Vector3 skinVertex in _groundSkinVertices)
        {
            float newXValue = Mathf.Cos(angle) * skinVertex.x - Mathf.Sin(angle) * skinVertex.z;
            float newZValue = Mathf.Cos(angle) * skinVertex.z + Mathf.Sin(angle) * skinVertex.x;
            Debug.DrawRay(_boxCollider.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue), Vector3.up * 2f, Color.red, 1f);


            RaycastHit hit;

            if (Physics.Raycast(_boxCollider.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue), Vector3.down, out hit, DistToGround, Layer))
            {
                BlockFaceBehaviour hitBlockFace = hit.collider.GetComponent<BlockFaceBehaviour>();

                if (!hitBlockFace.FireRaycastFromFace(0.1f, Layer, BlockFace.Top))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsMobile()
    {
        return _isMobile;
    }

    public void SetMobility(bool mobility)
    {
        _isMobile = mobility;
    }
}
