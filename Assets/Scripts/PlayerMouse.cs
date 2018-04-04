using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class should be renamed when control is finalized
public class PlayerMouse : MonoBehaviour
{
    public LayerMask CollidableLayer;
    private BlockFaceBehaviour _lastBlock;
    private HashSet<BlockFaceBehaviour> _faceMap = new HashSet<BlockFaceBehaviour>();
    private Vector3[] _directions = new Vector3[5]
        { Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

    private PlayerAnimatorController _animator;
    private PlayerController _playerController;

    void Awake()
    {
        _animator = GetComponentInChildren<PlayerAnimatorController>();
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        ResetFaces();

        if (Input.GetKeyDown(KeyCode.F) && _playerController.IsMobile())
        {
            HandleInteractForward();
        }
        else if (Input.GetKeyDown(KeyCode.V) && _playerController.IsMobile())
        {
            HandleInteractDownward();
        }
    }

    private void HandleInteractForward()
    {
        Vector3 GridSpaceCoordinate = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z));
        Ray ray = new Ray(GridSpaceCoordinate, transform.forward);
        RaycastHit hit;
        bool didRayCastHit = Physics.Raycast(ray, out hit, 0.6f, CollidableLayer);
        if (didRayCastHit)
        {
            BlockFaceBehaviour blockFace = hit.transform.gameObject.GetComponent<BlockFaceBehaviour>();
            if (blockFace == null) return;
            BlockFace face = BlockFaceMethods.BlockFaceFromNormal(hit.normal);

            _animator.AttemptToInteractWith(blockFace, face);
        }
    }

    private void HandleInteractDownward()
    {

        Vector3 GridSpaceCoordinate = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z));
        Ray ray = new Ray(GridSpaceCoordinate, Vector3.down);
        RaycastHit hit;
        bool didRayCastHit = Physics.Raycast(ray, out hit, 0.6f, CollidableLayer);
        if (didRayCastHit)
        {
            BlockFaceBehaviour blockFace = hit.transform.gameObject.GetComponent<BlockFaceBehaviour>();
            if (blockFace == null) return;
            BlockFace face = BlockFaceMethods.BlockFaceFromNormal(hit.normal);
      
            _animator.AttemptToInteractWith(blockFace, face);
        }
    }

    void ResetFaces()
    {
        foreach (BlockFaceBehaviour entry in _faceMap)
        {
            entry.UnhighlightFace();
        }

        _faceMap.Clear();
        SetNewFaces();
    }

    void SetNewFaces()
    {
        foreach (Vector3 direction in _directions)
        {
            GameObject blockGameObject = GetGameObjInDir(direction);
            BlockFace blockFaceOfNeighbouringBlock = BlockFaceMethods.BlockFaceFromNormal(-direction);

            if (blockGameObject == null) continue;

            BlockFaceBehaviour blockFaceBehaviour = blockGameObject.GetComponent<BlockFaceBehaviour>();
            BlockBehaviour blockBehaviour = blockGameObject.GetComponent<BlockBehaviour>();

            blockBehaviour.OnFaceSelect(blockFaceOfNeighbouringBlock);
            _faceMap.Add(blockFaceBehaviour);
        }
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
}
