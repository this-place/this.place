using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveablePlugin : BlockPlugin
{

    private bool _isDisplaced;
    private BlockFace _displacedFace;

    private void OnEnable()
    {
        _isDisplaced = false;
    }

    public override void OnFaceClick(BlockFace face)
    {
        if (_isDisplaced)
        {
            if (_block.MoveBlock(_displacedFace.GetOppositeFace()))
            {
                _isDisplaced = false;
                _displacedFace = BlockFace.Unknown;
            }
        }
        else
        {
            if (_block.MoveBlock(face))
            {
                _isDisplaced = true;
                _displacedFace = face;
            }
        }
    }
}
