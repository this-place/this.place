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
    private Vector3[] groundSkinVertices = new Vector3[40];

    // Use this for initialization
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _heading = Vector3.forward;
        groundSkinVertices[0] = Vector3.zero - (Vector3.right * (Bc.bounds.extents.x - 0.05f)) - (Vector3.forward * (Bc.bounds.extents.z - 0.05f));
        groundSkinVertices[1] = Vector3.zero - (Vector3.right * (Bc.bounds.extents.x - 0.05f)) + (Vector3.forward * (Bc.bounds.extents.z - 0.05f));
        groundSkinVertices[2] = Vector3.zero + (Vector3.right * (Bc.bounds.extents.x - 0.05f)) + (Vector3.forward * (Bc.bounds.extents.z - 0.05f));
        groundSkinVertices[3] = Vector3.zero + (Vector3.right * (Bc.bounds.extents.x - 0.05f)) - (Vector3.forward * (Bc.bounds.extents.z - 0.05f));
        for (int i = 0; i < 4; i++)
        {
            float xDiff;
            float zDiff;
            if (i < 3)
            {
                xDiff = groundSkinVertices[i + 1].x - groundSkinVertices[i].x;
                zDiff = groundSkinVertices[i + 1].z - groundSkinVertices[i].z;
            }
            else
            {
                xDiff = groundSkinVertices[0].x - groundSkinVertices[i].x;
                zDiff = groundSkinVertices[0].z - groundSkinVertices[i].z;
            }
            float zStart = groundSkinVertices[i].z;
            float xStart = groundSkinVertices[i].x;
            for (int j = 1; j < 10; j++)
            {
                groundSkinVertices[(j-1) + 4 + (9 * i)] = new Vector3(xStart + ((xDiff / 9) * j),
                    0,
                    zStart + ((zDiff / 9) * j));
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
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move();
        }

        if (Input.GetAxis("Jump") == 1.0f)
        {
            if (IsOnGround())
                Jump();
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(0, Input.GetAxis("Jump") * JumpForce, 0);
    }

    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = _right * MoveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = _forward * MoveSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (heading != Vector3.zero)
            transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    {
        {
        }
    }

    private bool IsOnGround()
    {
        float angle = Mathf.Atan2(_heading.x, _heading.z);
        foreach (Vector3 skinVertex in groundSkinVertices)
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
