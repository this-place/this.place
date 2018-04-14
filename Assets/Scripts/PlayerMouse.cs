using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class should be renamed when control is finalized
public class PlayerMouse : MonoBehaviour
{
    public LayerMask CollidableLayer;
    private BlockBehaviour _highlighted;
    private PlayerAnimatorController _animator;
    private PlayerController _playerController;

    void Awake()
    {
        _animator = GetComponentInChildren<PlayerAnimatorController>();
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        ResetFace();

        if (Input.GetAxisRaw("PullForward") == 1f && _playerController.IsMobile())
        {
            HandleInteractForward();
        }
    }

    private void HandleInteractForward()
    {
        Vector3 GridSpaceCoordinate = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z));
        Vector3 direction = SnapToCardinal(transform.forward);
        Ray ray = new Ray(GridSpaceCoordinate, direction);
        RaycastHit hit;
        bool didRayCastHit = Physics.Raycast(ray, out hit, 0.6f, CollidableLayer);
        if (didRayCastHit)
        {
            BlockBehaviour block = hit.transform.gameObject.GetComponent<BlockBehaviour>();
            if (block == null) return;
            BlockFace face = BlockFaceMethods.BlockFaceFromNormal(hit.normal);

            _animator.AttemptToInteractWith(block, face);
        }
    }

    void ResetFace()
    {
       if (_highlighted != null)
        {
            _highlighted.UnhighlightFace();
        }
        SetNewFace();
    }

    void SetNewFace()
    {
        Vector3 direction = SnapToCardinal(transform.forward);
        GameObject blockGameObject = GetGameObjInDir(direction);
        BlockFace blockFaceOfNeighbouringBlock = BlockFaceMethods.BlockFaceFromNormal(-direction);

        if (blockGameObject == null) return;

        BlockBehaviour blockBehaviour = blockGameObject.GetComponent<BlockBehaviour>();
        blockBehaviour.OnFaceSelect(blockFaceOfNeighbouringBlock);
        _highlighted = blockBehaviour;

    }

    GameObject GetGameObjInDir(Vector3 dir)
    {
        RaycastHit hit;

        Vector3 GridSpaceCoordinate = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z));

        Ray ray = new Ray(GridSpaceCoordinate, dir);
        if (Physics.Raycast(ray, out hit, 0.6f, CollidableLayer))
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    Vector3 SnapToCardinal(Vector3 direction)
    {
        float x = direction.x;
        float z = direction.z;

        if (Math.Abs(x) > Math.Abs(z))
        {
            return new Vector3(x, 0, 0).normalized;
        }
        else
        {
            return new Vector3(0, 0, z).normalized;
        }
    }
}
