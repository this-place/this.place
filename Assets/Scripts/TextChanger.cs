using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this was made because of issues with using TMPro
public class TextChanger : MonoBehaviour
{
    public Text Shadow;
    private Text Self;
    // Use this for initialization
    void Start()
    {
        Self = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Shadow.text = Self.text;
    }
}
