using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GravityPlugin : BlockPlugin {

    private const float Acceleration = 3f;

    public override void OnUpdate()
    {
        _block.MoveBlock(BlockFace.Bottom, Acceleration);
    }
}
