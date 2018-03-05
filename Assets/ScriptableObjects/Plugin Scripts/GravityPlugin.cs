using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GravityPlugin : BlockPlugin {

    public override void OnUpdate()
    {
        _block.MoveBlock(BlockFace.Bottom);
    }
}
