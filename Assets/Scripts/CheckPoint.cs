using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{

    public string Level;

    private Vector3 _position;

    private void Start()
    {
        _position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SendMessageUpwards("SetPosition", _position);
            SendMessageUpwards("SetLevel", Level);
        }
    }
}