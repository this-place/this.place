using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInfo : MonoBehaviour
{

    public GameObject Canvas;

    void Start()
    {
        Canvas.SetActive(false);
    }

    void Update()
    {
        if (Canvas.activeSelf)
        {
            Canvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Canvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Canvas.SetActive(false);
        }
    }
}
