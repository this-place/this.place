using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class FadeoutPlugin : BlockPlugin
{
    public const float FadeSpeed = 5f;
    public const float FadeMin = 0.0f;
    public const float FadeMax = 1.0f;
    private Renderer _renderer;
    private State _fadeState = State.Opaque;
    private CameraController _cameraController;

    public override void Plug(BlockBehaviour blockBehaviour)
    {
        base.Plug(blockBehaviour);
        _renderer = blockBehaviour.GetComponent<Renderer>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _block = blockBehaviour;
        _cameraController.AddFadeoutTarget(this);
    }

    public BlockBehaviour GetBlock()
    {
        return _block;
    }

    private void OnDestroy()
    {
        if (_cameraController != null)
        {
            _cameraController.RemoveFadeoutTarget(this);
        }
    }

    public override void OnUpdate()
    {
        float newAlpha;
        switch (_fadeState)
        {
            case State.Fading:
                newAlpha = Mathf.Clamp(_renderer.material.color.a - (FadeSpeed * Time.deltaTime), 0, 1);

                _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, newAlpha);

                if (newAlpha == 0)
                {
                    _fadeState = State.Transparent;
                }

                break;
            case State.Appearing:
                newAlpha = Mathf.Clamp(_renderer.material.color.a + (FadeSpeed * Time.deltaTime), 0, 1);
                _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, newAlpha);
                if (newAlpha == 1)
                {
                    _fadeState = State.Opaque;
                    _block.TransparentRenderer.SetTransparent(false);
                }
                break;
            case State.Transparent:
            case State.Opaque:
                break;
        }
    }

    public void SetIsFading(bool isFading)
    {
        if ((_fadeState == State.Opaque || _fadeState == State.Appearing) && isFading)
        {
            if (_fadeState == State.Opaque)
            {
                _block.TransparentRenderer.SetTransparent(true);
            }
            _fadeState = State.Fading;
        }

        if ((_fadeState == State.Transparent || _fadeState == State.Fading) && !isFading)
        {
            _fadeState = State.Appearing;
        }
    }

    private enum State
    {
        Fading,
        Appearing,
        Transparent,
        Opaque
    }
}
