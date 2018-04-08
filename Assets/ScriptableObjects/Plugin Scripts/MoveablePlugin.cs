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
        if (_isDisplaced) return;

        if (_block.MoveBlock(face))
        {
            _isDisplaced = true;
            _displacedFace = face;
            _block.PlayDisplacementSound();
        }
    }

    public override void OnFaceSelect(BlockFace face)
    {
        if (_isDisplaced) return;
        _block.HighlightFace(face);
    }

    public override Vector3 GetMoveDirection(BlockFace face)
    {
        if (_isDisplaced) return Vector3.zero;
        return face.GetNormal();
    }
}
