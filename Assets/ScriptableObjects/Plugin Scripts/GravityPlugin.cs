using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GravityPlugin : BlockPlugin
{
    public float TerminalSpeed = 10f;
    private const float Acceleration = 2f;
    private float _currentSpeed = 1f;

    public override void OnUpdate()
    {

        if (_block.Fall(_currentSpeed))
        {
            ResetSpeed();
        }
        else
        {
            UpdateSpeed();
        }

    }

    public void UpdateSpeed()
    {
        if (_block.CurrentSpeed < TerminalSpeed)
        {
            _currentSpeed += Time.deltaTime * Acceleration;
        }
        Debug.Log("increasing speed to " + _currentSpeed);
    }

    public void ResetSpeed()
    {
        _currentSpeed = _block.InitialSpeed;
        Debug.Log("resetting speed");
    }
}
