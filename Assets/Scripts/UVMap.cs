using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UVMap : MonoBehaviour, ITransparentRenderer
{

    public Material BlockMaterial;
    public Material TransparentMaterial;
    public bool useDefaultMapping;
    private Mesh _mesh;
    private Renderer _renderer;
    private Color _color;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = BlockMaterial;
        _mesh = GetComponent<MeshFilter>().mesh;
        _color = _renderer.material.GetColor("_Color");
        if (!useDefaultMapping)
        {
            SetUVs();
        }
    }


    public void SetNormalTexture()
    {
        if (!useDefaultMapping)
        {
            SetUVs();
        }
        else
        {
            _renderer.material.SetColor("_Color", _color);
        }
    }

    private void SetUVs()
    {
        Vector2[] blockUVs = (Vector2[])_mesh.uv.Clone();

        // Top
        blockUVs[4] = new Vector2(0.0f, 0.75f);
        blockUVs[5] = new Vector2(0.25f, 0.75f);
        blockUVs[8] = new Vector2(0.0f, 1.0f);
        blockUVs[9] = new Vector2(0.25f, 1.0f);

        // Bottom
        blockUVs[15] = new Vector2(0.0f, 0.5f);
        blockUVs[12] = new Vector2(0.25f, 0.5f);
        blockUVs[14] = new Vector2(0.0f, 0.75f);
        blockUVs[13] = new Vector2(0.25f, 0.75f);

        // North
        blockUVs[0] = new Vector2(0.25f, 0.75f);
        blockUVs[1] = new Vector2(0.5f, 0.75f);
        blockUVs[2] = new Vector2(0.25f, 1.0f);
        blockUVs[3] = new Vector2(0.5f, 1.0f);

        // South
        blockUVs[6] = new Vector2(0.5f, 0.75f);
        blockUVs[7] = new Vector2(0.75f, 0.75f);
        blockUVs[10] = new Vector2(0.5f, 1.0f);
        blockUVs[11] = new Vector2(0.75f, 1.0f);

        // East
        blockUVs[23] = new Vector2(0.25f, 0.5f);
        blockUVs[20] = new Vector2(0.5f, 0.5f);
        blockUVs[22] = new Vector2(0.25f, 0.75f);
        blockUVs[21] = new Vector2(0.5f, 0.75f);

        // West
        blockUVs[19] = new Vector2(0.5f, 0.5f);
        blockUVs[16] = new Vector2(0.75f, 0.5f);
        blockUVs[18] = new Vector2(0.5f, 0.75f);
        blockUVs[17] = new Vector2(0.75f, 0.75f);

        _mesh.uv = blockUVs;
    }

    public void SetFaceHighlight(BlockFace face)
    {
        if (!useDefaultMapping)
        {
            Vector2[] blockUVs = (Vector2[])_mesh.uv.Clone();

            switch (face)
            {
                case BlockFace.Top:
                    blockUVs[4] = new Vector2(0.0f, 0.25f);
                    blockUVs[5] = new Vector2(0.25f, 0.25f);
                    blockUVs[8] = new Vector2(0.0f, 0.5f);
                    blockUVs[9] = new Vector2(0.25f, 0.5f);
                    break;

                case BlockFace.Bottom:
                    blockUVs[15] = new Vector2(0.0f, 0.0f);
                    blockUVs[12] = new Vector2(0.25f, 0.0f);
                    blockUVs[14] = new Vector2(0.0f, 0.25f);
                    blockUVs[13] = new Vector2(0.25f, 0.25f);
                    break;

                case BlockFace.North:
                    blockUVs[0] = new Vector2(0.25f, 0.25f);
                    blockUVs[1] = new Vector2(0.5f, 0.25f);
                    blockUVs[2] = new Vector2(0.25f, 0.5f);
                    blockUVs[3] = new Vector2(0.5f, 0.5f);
                    break;

                case BlockFace.South:
                    blockUVs[6] = new Vector2(0.5f, 0.25f);
                    blockUVs[7] = new Vector2(0.75f, 0.25f);
                    blockUVs[10] = new Vector2(0.5f, 0.5f);
                    blockUVs[11] = new Vector2(0.75f, 0.5f);
                    break;

                case BlockFace.East:
                    blockUVs[23] = new Vector2(0.25f, 0.0f);
                    blockUVs[20] = new Vector2(0.5f, 0.0f);
                    blockUVs[22] = new Vector2(0.25f, 0.25f);
                    blockUVs[21] = new Vector2(0.5f, 0.25f);
                    break;

                case BlockFace.West:
                    blockUVs[19] = new Vector2(0.5f, 0.0f);
                    blockUVs[16] = new Vector2(0.75f, 0.0f);
                    blockUVs[18] = new Vector2(0.5f, 0.25f);
                    blockUVs[17] = new Vector2(0.75f, 0.25f);
                    break;
            }

            _mesh.uv = blockUVs;
        }
        else
        {
            _renderer.material.SetColor("_Color", Color.white);
        }
    }

    public void SetTransparent(bool transparency)
    {
        _renderer.shadowCastingMode = transparency ? ShadowCastingMode.Off : ShadowCastingMode.On;

        _renderer.material = transparency ? TransparentMaterial : BlockMaterial;
    }
}
