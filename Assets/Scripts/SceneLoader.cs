using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{

    public string Load;
    private bool _loaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_loaded && other.CompareTag("Player"))
        {
            _loaded = true;
            SendMessageUpwards("LoadNext");
        }
    }
}
