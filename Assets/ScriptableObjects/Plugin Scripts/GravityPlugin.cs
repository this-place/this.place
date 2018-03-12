using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GravityPlugin : BlockPlugin
{
    public float TerminalSpeed = 10f;
    private const float _acceleration = 5f;

    public override void OnUpdate()
    {
        if (!_block.IsTranslating())
        {
            bool didBlockMove = _block.MoveBlock(BlockFace.Bottom, _block.GetCurrentSpeed(), _acceleration);

            if (!didBlockMove)
            {
                // block could not move due to obstruction
                _block.SetCurrentSpeed(_block.InitialSpeed);
            }
        }
    }
}
