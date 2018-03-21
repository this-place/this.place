using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveablePlugin : BlockPlugin, IDisplaceable
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

    public bool DisplaceableInFaceDirection(BlockFace face)
    {
        if (_isDisplaced)
        {
            return face.Equals(_displacedFace);
        }
        return true;
    }
    
    public Vector3 GetDisplaceDirection(BlockFace face)
    {
        if (!_isDisplaced)
        {
            return face.GetNormal();
        }

        if (face.Equals(_displacedFace))
        {
            return -face.GetNormal();
        }

        throw new System.ArgumentException();
    }
}
