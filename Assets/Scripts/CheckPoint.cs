using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{

    //public string Level;
    public bool ClearFlags;
    public Color color;

    private Vector3 _position;
    

    private void Start()
    {
        _position = transform.position;
        SendMessageUpwards("RegisterCheckpoint", _position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SendMessageUpwards("UnloadPrevious");
            Camera.main.backgroundColor = color;
            if (ClearFlags)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
            else
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
        }
    }
}