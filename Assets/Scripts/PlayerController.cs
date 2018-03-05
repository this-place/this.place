using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private const float MoveSpeed = 2f;
    private const float JumpForce = 5.2f;
    private const float DistToGround = 0.5f;

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _heading;

    private Rigidbody _rb;
    public BoxCollider Bc;
    private Vector3[] _groundSkinVertices = new Vector3[40];
    private Vector3[] _forwardSkinVertices = new Vector3[16];
    private const float GroundSkinOffset = 0.5f;
    private const float ForwardSkinOffset = 0.1f;

    // Use this for initialization
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _heading = Vector3.forward;
        float groundXBound = Bc.bounds.extents.x - GroundSkinOffset;
        float groundZBound = Bc.bounds.extents.z - GroundSkinOffset;
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

        float forwardYBound = Bc.bounds.extents.y - ForwardSkinOffset;
        float forwardZBound = Bc.bounds.extents.z - ForwardSkinOffset;
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
        if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1 || Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            Move();
        }

        if (Input.GetAxis("Jump") == 1.0f && IsOnGround())
        {
                Jump();
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
            Debug.DrawRay(Bc.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue), transform.forward * 2f, Color.red);
            RaycastHit[] rayHits = Physics.RaycastAll(Bc.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue),
                transform.forward, movement.magnitude + Bc.bounds.extents.x);
            if (rayHits.Length != 0)
            {
                if (Physics.Raycast(Bc.bounds.center, transform.forward, 2f))
                    correctNormal = Physics.RaycastAll(Bc.bounds.center, transform.forward, 2f)[0].normal;
                else
                    correctNormal = rayHits[0].normal;
                Debug.Log(correctNormal);
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
            if (correctNormal != -transform.forward && (closestPoint - Bc.bounds.extents.x) < 0.01 && IsOnGround())
            {
                transform.forward = Vector3.Normalize(transform.forward - Vector3.Project(transform.forward, correctNormal));
                return CheckCollision(movement);
            }
            return (closestPoint - Bc.bounds.extents.x) * Vector3.Normalize(movement) * 0.95f;
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
            Debug.DrawRay(Bc.bounds.center + new Vector3(newXValue, skinVertex.y, newZValue), Vector3.up * 2f, Color.red, 1f);
            if (Physics.Raycast(Bc.bounds.center + 
                new Vector3(newXValue, skinVertex.y, newZValue), Vector3.down, DistToGround))
            {
                return true;
            }
        }

        return false;
    }
}
