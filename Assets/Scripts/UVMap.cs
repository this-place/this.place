using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UVMap : MonoBehaviour, ITransparentRenderer
{

    public Material BlockMaterial;
    public Material TransparentMaterial;
    private float x = 0;
    private float y = 1;
    private const float PixelSize = 2;
    private Mesh _mesh;
    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = BlockMaterial;
        _mesh = GetComponent<MeshFilter>().mesh;
        SetUVs();
    }


    public void SetHighlightTexture()
    {
        x = 1;
        SetUVs();
    }

    public void SetNormalTexture()
    {
        x = 0;
        SetUVs();
    }

    private void SetUVs()
    {
        float tilePerc = 1 / PixelSize;

        float umin = tilePerc * x;
        float umax = tilePerc * (x + 1);
        float vmin = tilePerc * y;
        float vmax = tilePerc * (y + 1);

        Vector2[] blockUVs = new Vector2[24];

        blockUVs[0] = new Vector2(umin, vmin);
        blockUVs[1] = new Vector2(umax, vmin);
        blockUVs[2] = new Vector2(umin, vmax);
        blockUVs[3] = new Vector2(umax, vmax);
        blockUVs[4] = new Vector2(umin, vmax);
        blockUVs[5] = new Vector2(umax, vmax);
        blockUVs[6] = new Vector2(umin, vmax);
        blockUVs[7] = new Vector2(umax, vmax);
        blockUVs[8] = new Vector2(umin, vmin);
        blockUVs[9] = new Vector2(umax, vmin);
        blockUVs[10] = new Vector2(umin, vmin);
        blockUVs[11] = new Vector2(umax, vmin);
        blockUVs[12] = new Vector2(umin, vmin);
        blockUVs[13] = new Vector2(umax, vmax);
        blockUVs[14] = new Vector2(umin, vmax);
        blockUVs[15] = new Vector2(umax, vmin);
        blockUVs[16] = new Vector2(umin, vmin);
        blockUVs[17] = new Vector2(umin, vmax);
        blockUVs[18] = new Vector2(umax, vmax);
        blockUVs[19] = new Vector2(umax, vmin);
        blockUVs[20] = new Vector2(umin, vmin);
        blockUVs[21] = new Vector2(umax, vmax);
        blockUVs[22] = new Vector2(umin, vmax);
        blockUVs[23] = new Vector2(umax, vmin);

        _mesh.uv = blockUVs;
    }

    public void SetFaceHighlight(BlockFace face)
    {
        Vector2[] blockUVs = (Vector2[])_mesh.uv.Clone();

        float tilePerc = 1 / PixelSize;
        float umin = tilePerc;
        float umax = tilePerc * 2;
        float vmin = tilePerc * y;
        float vmax = tilePerc * (y + 1);
        Vector2 minmin = new Vector2(umin, vmin);
        Vector2 maxmin = new Vector2(umax, vmin);
        Vector2 minmax = new Vector2(umin, vmax);
        Vector2 maxmax = new Vector2(umax, vmax);
        switch (face)
        {
            case BlockFace.Top:
                blockUVs[4] = minmin;
                blockUVs[5] = maxmin;
                blockUVs[8] = minmax;
                blockUVs[9] = maxmax;
                break;
            case BlockFace.Bottom:
                blockUVs[12] = minmax;
                blockUVs[13] = maxmin;
                blockUVs[14] = maxmax;
                blockUVs[15] = minmin;
                break;
            case BlockFace.North:
                blockUVs[0] = minmax;
                blockUVs[1] = maxmax;
                blockUVs[2] = minmin;
                blockUVs[3] = maxmin;
                break;
            case BlockFace.South:
                blockUVs[6] = minmin;
                blockUVs[7] = maxmin;
                blockUVs[10] = minmax;
                blockUVs[11] = maxmax;
                break;
            case BlockFace.East:
                blockUVs[20] = minmax;
                blockUVs[21] = maxmin;
                blockUVs[22] = maxmax;
                blockUVs[23] = minmin;
                break;
            case BlockFace.West:
                blockUVs[16] = minmax;
                blockUVs[17] = maxmin;
                blockUVs[18] = maxmax;
                blockUVs[19] = minmin;
                break;
        }

        _mesh.uv = blockUVs;
    }

    public void SetTransparent(bool transparency)
    {
        _renderer.shadowCastingMode = transparency ? ShadowCastingMode.Off : ShadowCastingMode.On;

        _renderer.material = transparency ? TransparentMaterial : BlockMaterial;
    }
}
