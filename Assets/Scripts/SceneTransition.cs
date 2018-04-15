using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public Material SkyBox;
    public float Intensity = 1f;
    private bool _triggered;
    private Light _light;

    private void Start()
    {
        _light = GameObject.Find("Directional Light").GetComponent<Light>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderSettings.skybox = SkyBox;
            _triggered = true;
        }
    }

    private void Update()
    {
        if (_triggered)
        {
            if (Mathf.Abs(Intensity - _light.intensity) < 0.1f)
            {
                _light.intensity += Mathf.Sign(Intensity - _light.intensity) * Time.deltaTime * 1f;
            }
            else
            {
                _light.intensity = Intensity;
                _triggered = false;
            }
        }
    }

}