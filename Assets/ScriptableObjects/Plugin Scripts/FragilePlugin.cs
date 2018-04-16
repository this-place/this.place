using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FragilePlugin : BlockPlugin
{
    public float BreakAfterTime;
    public ParticleSystem FragileEmitter;

    private bool _isDestroyingItself;
    private ParticleSystem _fragileEmitterInstantiated;

    public override void OnUpdate()
    {
        if (!_isDestroyingItself && (_block.IsTranslating() || _block.IsPlayerStandingOn()))
        {
            _fragileEmitterInstantiated = Instantiate(FragileEmitter, _block.gameObject.transform.position + Vector3.down, _block.gameObject.transform.rotation) as ParticleSystem;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_fragileEmitterInstantiated.gameObject, _block.gameObject.scene);
            _fragileEmitterInstantiated.GetComponent<ParticleSystemRenderer>().material = _block.gameObject.GetComponent<Renderer>().material;
            _fragileEmitterInstantiated.Play();
            Destroy(_fragileEmitterInstantiated.gameObject, BreakAfterTime + 1.5f);
            Destroy(_block.gameObject, BreakAfterTime);
            _isDestroyingItself = true;
        }

        if (_isDestroyingItself && _block.IsTranslating())  // To make the emitter follow the block
        {
            _fragileEmitterInstantiated.gameObject.transform.position = _block.gameObject.transform.position;
            _fragileEmitterInstantiated.gameObject.transform.rotation = _block.gameObject.transform.rotation;
        }
    }
}

