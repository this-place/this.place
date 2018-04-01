using UnityEngine;
using System.Collections;

public class ScrollBehaviour : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";
    private Renderer _renderer;

    Vector2 uvOffset = Vector2.zero;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (_renderer.enabled)
        {
            _renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }
}
