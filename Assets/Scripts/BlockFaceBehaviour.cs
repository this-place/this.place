using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

public class BlockFaceBehaviour : MonoBehaviour
{

    public BlockBehaviour Block;

    private Color _originalColor;
    private Renderer _renderer;

    private Vector3 _normal;
    private float _faceLength;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] localVertices = mesh.vertices;
        Assert.AreEqual(4, localVertices.Length);

        // convert local vertex into world space coordinate
        Vector3[] worldCoord = new Vector3[localVertices.Length];
        for (int i = 0; i < localVertices.Length; i++)
        {
            worldCoord[i] = transform.TransformPoint(localVertices[i]);

        }

        // Choice of which coordinates to use was done via trial and error...
        _faceLength = Vector3.Distance(worldCoord[0], worldCoord[2]);
        _normal = CalculateFaceNormal(worldCoord);
    }

    private static Vector3 CalculateFaceNormal(Vector3[] worldCoords)
    {
        // Unity uses clockwise winding order for face normals
        // https://forum.unity.com/threads/unity-has-a-clockwise-winding-order.129923/
        Vector3 sideA = worldCoords[1] - worldCoords[0];
        Vector3 sideB = worldCoords[2] - worldCoords[0];

        return Vector3.Cross(sideA, sideB);
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

        Block.MoveBlock(_normal, _faceLength);
    }
}
