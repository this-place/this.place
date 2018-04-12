using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour
{
    public int OptionalNumber = 0;
    private float _rotateSpeed = 0.3f;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, _rotateSpeed);
    }

    private void OnTriggerEnter(Collider colider)
    {
        if (colider.CompareTag("Player"))
        {
            SendMessageUpwards("UpdateScore", this);
            Destroy(gameObject);
        }
    }

    public void SetCollected()
    {
        GetComponent<Renderer>().material.color = Color.grey;
    }
}
