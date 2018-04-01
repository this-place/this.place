using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimatorController : MonoBehaviour
{

    private Animator _animator;

    private PlayerController _playerController;

    private BlockFaceBehaviour _blockFace;
    private BlockFace _face;
    private Vector3 _direction;
    private bool _isMovingTowardsBlock = false;
    private Vector3 _blockFacePosition;
    private bool _isMovingBlock = false;
    private Scene _originalScene;

    // Use this for initialization
    private void Start()
    {
        _originalScene = gameObject.scene;
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (_isMovingTowardsBlock)
        {
            HandleMoveTowardsBlock();
        }

        if (_isMovingBlock && _blockFace == null)
        {
            _playerController.SetMobility(true);
            _playerController.SetGravity(true);
            transform.parent.parent = null;
            _isMovingBlock = false;
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
        _isMovingBlock = false;

       UnParentPlayer();
    }

    public void UnParentPlayer()
    {
        // set character to its original scene
        transform.parent.parent = null;
        SceneManager.MoveGameObjectToScene(transform.parent.gameObject, _originalScene);
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
                if (_face.GetNormal() == _direction)
                {
                    _animator.SetTrigger("Pull Up");
                }
                else
                {
                    _animator.SetTrigger("Push Down");
                }
            }
            else
            {
                LookAtBlock();
                if (_face.GetNormal() == _direction)
                {
                    _animator.SetTrigger("Pull Back");
                }
                else
                {
                    _animator.SetTrigger("Push Back");
                }

            }
            // player will translate with block
            _isMovingBlock = true;
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

    public void MoveBlock(BlockFaceBehaviour blockFaceBehaviour, BlockFace face, Vector3 direction)
    {
        _blockFace = blockFaceBehaviour;
        _face = face;
        _direction = direction;

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

    public void Jump()
    {
        _animator.ResetTrigger("Ground");
        _animator.SetTrigger("Jump");
    }

    public void Ground()
    {
        if (_animator == null) return;
        _animator.SetTrigger("Ground");
    }
}
