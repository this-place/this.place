using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour
{
    public ParticleSystem CollectibleEmitter;
    public AudioSource CollectibleCollectedSound;

    public int OptionalNumber = 0;

    private float _rotateSpeed = 0.6f;
    private float _floatSpeed = 0.02f;
    private ParticleSystem _collectibleEmitterInstantiated;
    private bool _collected = false;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, _rotateSpeed);
        if (_collected)
        {
            transform.position += new Vector3(0, _floatSpeed, 0);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !_collected)
        {
            SendMessageUpwards("UpdateScore", this);
            _rotateSpeed += 10f;
            Destroy(gameObject, 1f);
            _collected = true;
            _collectibleEmitterInstantiated = Instantiate(CollectibleEmitter, transform.position, Quaternion.identity) as ParticleSystem;
            _collectibleEmitterInstantiated.GetComponent<ParticleSystemRenderer>().material.color = GetComponent<Renderer>().material.color;
            _collectibleEmitterInstantiated.Play();
            if (CollectibleCollectedSound.clip != null)
            {
                AudioSource.PlayClipAtPoint(CollectibleCollectedSound.clip, transform.position);
            }
            Destroy(_collectibleEmitterInstantiated, 1.5f);
        }
    }

    public void SetCollected()
    {
        GetComponent<Renderer>().material.color = Color.grey;
    }
}
