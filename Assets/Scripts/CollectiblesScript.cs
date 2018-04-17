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
    private float _rotateAmount = 0;

    private void Awake()
    {
        StartCoroutine(SpawnCollectible());
    }

    IEnumerator SpawnCollectible()
    {
        if (gameObject.scene.name == "Tutorial" || gameObject.scene.name == "Main")
        {
            yield break;
        }

        LensFlare lens = GetComponent<LensFlare>();
        float originalBrightness = lens.brightness;
        lens.brightness = 0;
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("CheckPoint");
        GameObject checkpoint = gameObject;
       
        foreach (GameObject cp in checkpoints)
        {
            if (cp.gameObject.scene == gameObject.scene)
            {
                checkpoint = cp;
                break;
            }
        }

        Vector3 checkPointTransform = checkpoint.transform.position;
        Vector3 collectibleTransform = transform.position;
        checkPointTransform.y = 0;
        collectibleTransform.y = 0;
        float timeToWait = Mathf.FloorToInt(Vector3.Distance(checkPointTransform, collectibleTransform));
        yield return new WaitForSeconds(timeToWait * 0.2f);
        float t = 0;
        while (t < 1)
        {
            t += 5 * Time.deltaTime;
            transform.localScale = originalScale * t;
            yield return null;
        }

        transform.localScale = originalScale;
        lens.brightness = originalBrightness;
    }

    public void DeSpawn(Vector3 position)
    {
        StartCoroutine(DeSpawnCollectible(position));
    }

    IEnumerator DeSpawnCollectible(Vector3 position)
    {
        float timeToWait = Mathf.FloorToInt(Vector3.Distance(position, transform.position));
        yield return new WaitForSeconds(timeToWait * 0.2f);
        Vector3 originalScale = transform.localScale;
        float t = 1;
        while (t > 0)
        {
            t -= 5 * Time.deltaTime;
            transform.localScale = originalScale * t;
            yield return null;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, _rotateSpeed);
        if (_collected)
        {
            transform.position += new Vector3(0, _floatSpeed, 0);
        }
        _rotateAmount += _rotateSpeed;
    }

    public void ResetRotation()
    {
        transform.RotateAround(transform.position, Vector3.up, -_rotateAmount);
        _rotateAmount = 0;
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
