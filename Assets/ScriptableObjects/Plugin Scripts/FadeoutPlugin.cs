using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class FadeoutPlugin : BlockPlugin
{
    public const float FadeSpeed = 0.1f;
    public const float FadeMin = 0.0f;
    public const float FadeMax = 1.0f;
    private Renderer _renderer;
    public bool IsFading = false;

    public override void Plug(BlockBehaviour blockBehaviour)
    {
        base.Plug(blockBehaviour);
        _renderer = blockBehaviour.GetComponent<Renderer>();
        _renderer.shadowCastingMode = ShadowCastingMode.Off;
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        _block = blockBehaviour;
        cameraController.AddFadeoutTarget(this);
    }

    public BlockBehaviour GetBlock()
    {
        return _block;
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (_renderer.material.color.a > FadeMin && !IsFading)
	        _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _renderer.material.color.a - FadeSpeed);

        if (_renderer.material.color.a < FadeMax && IsFading)
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _renderer.material.color.a + FadeSpeed);
    }
}
