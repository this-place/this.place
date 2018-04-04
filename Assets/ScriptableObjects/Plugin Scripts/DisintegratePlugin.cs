using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DisintegratePlugin : BlockPlugin
{
    private bool _isDisplaced;
    private BlockFace _directionOfTravel;

    public float TerminalSpeed = 10f;
    private const float Acceleration = 30f;
    private const float GravityAcceleration = 5f;

    private Animator _animator;

    public override void Plug(BlockBehaviour blockBehaviour)
    {
        base.Plug(blockBehaviour);
        _isDisplaced = false;
        _directionOfTravel = BlockFace.Unknown;
        _block = blockBehaviour;
        _animator = blockBehaviour.GetComponent<Animator>();
    }

    public override void OnFaceClick (BlockFace face)
    {
        if (!_isDisplaced && face != BlockFace.Top)
        {
            if (_block.MoveBlock(face))
            {
                _isDisplaced = true;
                _directionOfTravel = face.GetOppositeFace();
            }
        }

        _animator.SetTrigger("BlockPulled");
    }

    public override void OnUpdate()
    {
        if (!_block.IsTranslating() && _isDisplaced)
        {
            bool didBlockMove = _block.MoveBlock(_directionOfTravel, _block.GetCurrentSpeed(), Acceleration);

            if (!didBlockMove)
            {
                // block could not move due to obstruction
                _block.SetCurrentSpeed(_block.InitialSpeed);
                _block.SelfDestruct();

                // fire raycast to grab collidable reference
                GameObject collidedBlock = _block.GetCollidableObject(_directionOfTravel);

                if (collidedBlock != null)
                {
                    Destroy(collidedBlock);
                }
            }
        }
        else if (!_block.IsTranslating())
        {
            bool didBlockMove = _block.MoveBlock(BlockFace.Bottom, _block.GetCurrentSpeed(), GravityAcceleration);

            if (!didBlockMove)
            {
                // block could not move due to obstruction
                _block.SetCurrentSpeed(_block.InitialSpeed);
            }
        }
    }


    public override void OnFaceSelect(BlockFace face)
    {
        if (_isDisplaced || face.Equals(BlockFace.Top)) return;
        _block.HighlightFace(face);
    }

    public override Vector3 GetMoveDirection(BlockFace face)
    {
        if (_isDisplaced || face.Equals(BlockFace.Top))
        {
            // use Vector3.zero as error value
            return Vector3.zero;
        }
        return face.GetNormal();
    }
}
