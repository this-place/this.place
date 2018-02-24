using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

public class BlockFaceBehaviour : MonoBehaviour
{

    public BlockBehaviour Block;
    public LayerMask IgnoreCollision;

    private Color _originalColor;
    private Renderer _renderer;
    private Mesh _mesh;

    private Vector3 _normal;
    private float _faceLength;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        _mesh = meshFilter.mesh;

        // convert local vertex into world space coordinate
        Vector3[] worldCoord = GetWorldCoordinates();

        // quad vertices are in this order:
        // 3 --- 1 
        // |     | with the normal facing you
        // 0 --- 2
        _faceLength = Vector3.Distance(worldCoord[0], worldCoord[2]);
        _normal = CalculateFaceNormal(worldCoord);
    }

    public Vector3 GetNormal()
    {
        return _normal;
    }

    public float GetFaceLength()
    {
        return _faceLength;
    }

    public bool FireRaycastFromFace(float skinToLengthRatio, LayerMask ignores)
    {
        float skinWidth = skinToLengthRatio * _faceLength;   

        // convert local vertex into world space coordinate
        Vector3[] worldCoord = GetWorldCoordinates();

        // quad vertices are in this order:
        // 3 -C- 1 
        // B     D with the normal facing you
        // 0 -A- 2
        // 
        Vector3 a20 = worldCoord[0] - worldCoord[2];
        Vector3 b03 = worldCoord[3] - worldCoord[0];
        Vector3 c31 = worldCoord[1] - worldCoord[3];
        Vector3 d12 = worldCoord[2] - worldCoord[1];

        // we are firing our ray cast from within the cube
        Vector3[] skinVertices = new Vector3[4];
        skinVertices[0] = worldCoord[0] - (a20 * skinWidth) + (b03 * skinWidth) - (_normal * skinWidth);
        skinVertices[1] = worldCoord[1] - (c31 * skinWidth) + (d12 * skinWidth) - (_normal * skinWidth);
        skinVertices[2] = worldCoord[2] + (a20 * skinWidth) + (b03 * skinWidth) - (_normal * skinWidth);
        skinVertices[3] = worldCoord[3] + (c31 * skinWidth) + (d12 * skinWidth) - (_normal * skinWidth);

        foreach (Vector3 skinVertex in skinVertices)
        {
            Debug.DrawRay(skinVertex, _normal * 0.5f, Color.red, 1f);
            if (Physics.Raycast(skinVertex, _normal, _faceLength, ignores))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3[] GetWorldCoordinates()
    {
        Vector3[] localVertices = _mesh.vertices;
        Assert.AreEqual(4, localVertices.Length);
        Vector3[] worldCoord = new Vector3[localVertices.Length];
        for (int i = 0; i < localVertices.Length; i++)
        {
            worldCoord[i] = transform.TransformPoint(localVertices[i]);
        }

        return worldCoord;
    }

    private static Vector3 CalculateFaceNormal(Vector3[] worldCoords)
    {
        // Unity uses clockwise winding order for face normals
        // https://forum.unity.com/threads/unity-has-a-clockwise-winding-order.129923/
        Vector3 sideA = worldCoords[1] - worldCoords[0];
        Vector3 sideB = worldCoords[2] - worldCoords[0];

        return Vector3.Cross(sideA, sideB).normalized;
    }

    private void OnMouseEnter()
    {
        // TODO: Expose color to inspector
        _renderer.material.color = Color.black;
    }

    private void OnMouseExit()
    {
        _renderer.material.color = _originalColor;  
    }

    private void OnMouseDown()
    {
        Block.MoveBlock(this);
    }
}
