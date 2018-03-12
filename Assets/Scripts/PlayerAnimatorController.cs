using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    private Animator _animator;

    private PlayerController _playerController;

    private BlockFaceBehaviour _blockFace;
    private BlockFace _face;
	// Use this for initialization
	void Start ()
	{
	    _animator = GetComponent<Animator>();
	    _playerController = GetComponentInParent<PlayerController>();
	}
	
    void OnMoveBlockEnd()
    {
        _playerController.SetMobility(true);
    }

    void OnMoveBlockStart()
    {
        _blockFace.MoveClickedFace(_face);
    }

    public void MoveBlock(BlockFaceBehaviour blockFaceBehaviour, BlockFace face)
    {
        if (face == BlockFace.Top)
        {
            _animator.SetTrigger("Pull Up");
        }
        else
        {
            _animator.SetTrigger("Pull Back");
        }

        _blockFace = blockFaceBehaviour;
        _face = face;
        _playerController.SetMobility(false);
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
