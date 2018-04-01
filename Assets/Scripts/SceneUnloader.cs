using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnloader : MonoBehaviour
{

    public string Unload;
    private bool _unloaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_unloaded && other.CompareTag("Player"))
        {
            SendMessageUpwards("Unload", Unload);
            _unloaded = true;
        }
    }
}
