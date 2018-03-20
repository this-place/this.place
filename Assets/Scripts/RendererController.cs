using UnityEngine;
using UnityEngine.Rendering;

public class RendererController : MonoBehaviour, ITransparentRenderer
{
    public Material TransparentMaterial;
    private Material _originalMaterial;
    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
    }


    public void SetTransparent(bool transparency)
    {
        _renderer.shadowCastingMode = transparency ? ShadowCastingMode.Off : ShadowCastingMode.On;

        _renderer.material = transparency ? TransparentMaterial : _originalMaterial;
    }
}
