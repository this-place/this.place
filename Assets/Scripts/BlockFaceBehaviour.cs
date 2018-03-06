using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

public class BlockFaceBehaviour : MonoBehaviour
{
    public float FaceLength;

    private BlockBehaviour _block;
    private Color _originalColor;
    private Renderer _renderer;
    private Mesh _mesh;

    private Vector3 _normal;

    private void Start()
    {
        _block = GetComponent<BlockBehaviour>();
    }

    public float GetFaceLength()
    {
        return FaceLength;
    }

    public bool FireRaycastFromFace(float skinToLengthRatio, LayerMask collidableLayers, BlockFace face)
    {
        float centerToSkin = FaceLength * (1 - skinToLengthRatio) / 2;
        Vector3 normal = face.GetNormal();
        Vector3[] perpendicularNormals = face.GetPerpendicularNormals();
        Vector3 quadCenter = transform.position + (centerToSkin * normal);

        Vector3[] skinVertices = new Vector3[4];
        Vector3 scaledPNormal1 = centerToSkin * perpendicularNormals[0];
        Vector3 scaledPNormal2 = centerToSkin * perpendicularNormals[1];

        skinVertices[0] = quadCenter + scaledPNormal1 + scaledPNormal2;
        skinVertices[1] = quadCenter + scaledPNormal1 - scaledPNormal2;
        skinVertices[2] = quadCenter - scaledPNormal1 + scaledPNormal2;
        skinVertices[3] = quadCenter - scaledPNormal1 - scaledPNormal2;

        foreach (Vector3 skinVertex in skinVertices)
        {
            Debug.DrawRay(skinVertex, normal * FaceLength, Color.red, 1f);
            if (Physics.Raycast(skinVertex, normal, FaceLength, collidableLayers))
            {
                return true;
            }
        }
        return false;
    }

    public void OnMouseClick(Vector3 normal)
    {
        BlockFace clickedFace = BlockFaceMethods.BlockFaceFromNormal(normal);
        _block.OnFaceClick(clickedFace);
    }
}




