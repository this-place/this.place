using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    public float offsetAngle = 0;
    private Vector3 _position;
    private void Start()
    {
        _position = transform.position;
        SendMessageUpwards("RegisterCheckpoint", _position);
        SendMessageUpwards("UpdateCheckpointOffset", offsetAngle);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SendMessageUpwards("UnloadPrevious");
        }
    }
}