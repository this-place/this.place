using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BlockFaceBehaviour : MonoBehaviour
{
    public float FaceLength;

    public BlockBehaviour _block;
    private UVMap _uvMap;
    private Color _originalColor;
    private Renderer _renderer;
    private Mesh _mesh;

    private Vector3 _normal;

    private void Awake()
    {
        _block = GetComponent<BlockBehaviour>();
        _uvMap = GetComponent<UVMap>();
    }

    public float GetFaceLength()
    {
        return FaceLength;
    }

    public Vector3[] GetSkinVertices(float skinToLengthRatio, LayerMask collidableLayers, Vector3 normal, BlockFace face)
    {
        float centerToSkin = FaceLength * (1 - skinToLengthRatio) / 2;
        Vector3[] perpendicularNormals = face.GetPerpendicularNormals();
        Vector3 quadCenter = transform.position + (centerToSkin * normal);

        Vector3[] skinVertices = new Vector3[4];
        Vector3 scaledPNormal1 = centerToSkin * perpendicularNormals[0];
        Vector3 scaledPNormal2 = centerToSkin * perpendicularNormals[1];

        skinVertices[0] = quadCenter + scaledPNormal1 + scaledPNormal2;
        skinVertices[1] = quadCenter + scaledPNormal1 - scaledPNormal2;
        skinVertices[2] = quadCenter - scaledPNormal1 + scaledPNormal2;
        skinVertices[3] = quadCenter - scaledPNormal1 - scaledPNormal2;

        return skinVertices;
    }

    public bool FireRaycastFromFace(float skinToLengthRatio, LayerMask collidableLayers, BlockFace face)
    {
        Vector3 normal = face.GetNormal();
        Vector3[] skinVertices = GetSkinVertices(skinToLengthRatio, collidableLayers, normal, face);

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

    public GameObject GetRaycastObjectRef(float skinToLengthRatio, LayerMask collidableLayers, BlockFace face)
    {
        Vector3 normal = face.GetNormal();
        Vector3[] skinVertices = GetSkinVertices(skinToLengthRatio, collidableLayers, normal, face);

        foreach (Vector3 skinVertex in skinVertices)
        {
            RaycastHit hit;

            if (Physics.Raycast(skinVertex, normal, out hit))
            {
                return hit.collider.gameObject;
            }

        }
        return null;
    }

    public void MoveClickedFace(BlockFace clickedFace)
    {
        _block.OnFaceClick(clickedFace);
    }

    public void HighlightFace(BlockFace face)
    {
        if (_uvMap != null)
            _uvMap.SetFaceHighlight(face);
    }

    public void UnhighlightFace()
    {
        if (_uvMap != null)
            _uvMap.SetNormalTexture();
    }
}
