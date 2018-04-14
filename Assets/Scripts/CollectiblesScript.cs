using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour
{
    public ParticleSystem CollectibleEmitter;
    public int OptionalNumber = 0;
    private float _rotateSpeed = 0.3f;
    private float _floatSpeed = 0;
    private ParticleSystem _collectibleEmitterInstantiated;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, _rotateSpeed);
        transform.position += new Vector3(0, _floatSpeed, 0);
    }

    private void OnTriggerEnter(Collider colider)
    {
        if (colider.CompareTag("Player"))
        {
            SendMessageUpwards("UpdateScore", this);
            _rotateSpeed += 10f;
            _floatSpeed = 0.02f;
            Destroy(gameObject, 1f);

            _collectibleEmitterInstantiated = Instantiate(CollectibleEmitter, transform.position, Quaternion.identity) as ParticleSystem;
            _collectibleEmitterInstantiated.GetComponent<ParticleSystemRenderer>().material.color = GetComponent<Renderer>().material.color;
            _collectibleEmitterInstantiated.Play();
            Destroy(_collectibleEmitterInstantiated, 1.5f);
        }
    }

    public void SetCollected()
    {
        GetComponent<Renderer>().material.color = Color.grey;
    }
}
