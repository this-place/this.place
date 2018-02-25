using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveablePlugin : BlockPlugin
{

    private bool _isDisplaced;
    private BlockFaceBehaviour _displacedFace;

    private void OnEnable()
    {
        _isDisplaced = false;
    }

    public override void OnFaceClick(BlockFaceBehaviour face)
    {
        if (_isDisplaced)
        {
            if (_block.MoveBlock(_block.GetOppositeFace(_displacedFace)))
            {
                _isDisplaced = false;
                _displacedFace = null;
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
