using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour
{
    public int optionalNumber = 0;
    private float rotateSpeed = 0.3f;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotateSpeed);
    }

    private void OnTriggerEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
