using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    private Animator _animator;

    private PlayerController _playerController;

    private BlockFaceBehaviour _blockFace;
    private BlockFace _face;
    private bool _isMovingTowardsBlock = false;
    private Vector3 _blockFacePosition;

    // Use this for initialization
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (_isMovingTowardsBlock)
        {
            HandleMoveTowardsBlock();
        }
    }

    private void OnMoveBlockStart()
    {
        _blockFace.MoveClickedFace(_face);
    }

    private void OnMoveBlockEnd()
    {
        _playerController.SetMobility(true);
        _playerController.SetGravity(true);
        transform.parent.parent = null;
    }

    private void HandleMoveTowardsBlock()
    {
        _blockFacePosition.y = transform.position.y;
        Vector3 playerToBlock = _blockFacePosition - transform.position;
        Vector3 dir = playerToBlock.normalized;
        _playerController.MoveInDir(dir);

        if (Vector3.Distance(transform.position, _blockFacePosition) <= 0.1f)
        {
            // we have reached the block face, time to move it
            _isMovingTowardsBlock = false;
            if (_face == BlockFace.Top)
            {
                _animator.SetTrigger("Pull Up");
            }
            else
            {
                LookAtBlock();
                _animator.SetTrigger("Pull Back");
            }
            // player will translate with block
            SetAsChildOfBlock();
        }
    }

    private void SetAsChildOfBlock()
    {
        transform.parent.transform.parent = _blockFace.transform;
    }

    private void LookAtBlock()
    {
        float angle = Vector3.SignedAngle(transform.forward, -_face.GetNormal(), Vector3.up);
        transform.parent.Rotate(Vector3.up, angle);
    }

    public void MoveBlock(BlockFaceBehaviour blockFaceBehaviour, BlockFace face)
    {
        _blockFace = blockFaceBehaviour;
        _face = face;

        _playerController.SetMobility(false);
        _playerController.SetGravity(false);

        _isMovingTowardsBlock = true;
        _blockFacePosition = blockFaceBehaviour.transform.position + face.GetNormal() * (0.5f + _playerController.GetBoxCollider().bounds.extents.z);
    }

    public void MovePlayer()
    {
        _animator.SetBool("IsMoving", true);
    }

    public void StopPlayer()
    {
        _animator.SetBool("IsMoving", false);
    }
}
