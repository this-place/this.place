using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVMap : MonoBehaviour
{

    public Material material;

    // Current texture coordinates:
    // No highlight: (x,y) == (0,1)
    // Highlight: (x,y) == (1,1)
    //
    // TODO: Change coordinate system for 6 faces mapping
    private float x = 0;
    private float y = 1;
    private const float PixelSize = 2;

    void Start()
    {
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
        blockUVs[17] = new Vector2(umax, vmax);
        blockUVs[18] = new Vector2(umin, vmax);
        blockUVs[19] = new Vector2(umax, vmin);
        blockUVs[20] = new Vector2(umin, vmin);
        blockUVs[21] = new Vector2(umax, vmax);
        blockUVs[22] = new Vector2(umin, vmax);
        blockUVs[23] = new Vector2(umax, vmin);

        this.GetComponent<Renderer>().material = material;
        this.GetComponent<MeshFilter>().mesh.uv = blockUVs;
    }
}
