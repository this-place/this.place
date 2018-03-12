using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    public LayerMask CollidableLayer;

    private BlockFaceBehaviour _lastBlock;
    private Dictionary<BlockFaceBehaviour, BlockFace> _faceMap = new Dictionary<BlockFaceBehaviour, BlockFace>();
    private Vector3[] _directions = new Vector3[5]
        { Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

    private PlayerAnimatorController _animator;
    private PlayerController _playerController;

    void Start()
    {
        _animator = GetComponentInChildren<PlayerAnimatorController>();
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        ResetFaces();

        if (Input.GetMouseButtonDown(0) && _playerController.IsMobile())
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool didRayCastHit = Physics.Raycast(ray, out hit);

        // https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        // looking for block collisions only
        bool isHitTargetInCollidableLayer =
            didRayCastHit ? CollidableLayer == (CollidableLayer | (1 << hit.transform.gameObject.layer)) : false;

        if (didRayCastHit && isHitTargetInCollidableLayer)
        {
            BlockFaceBehaviour blockFace = hit.transform.gameObject.GetComponent<BlockFaceBehaviour>();
            BlockFace face = BlockFaceMethods.BlockFaceFromNormal(hit.normal);
            if (_faceMap.ContainsKey(blockFace) && face == _faceMap[blockFace])
            {
                _animator.MoveBlock(blockFace, face);
            }
        }
    }

    void ResetFaces()
    {
        foreach (KeyValuePair<BlockFaceBehaviour, BlockFace> entry in _faceMap)
        {
            entry.Key.UnhighlightFace();
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

            if (blockBehaviour.IsTranslating()) continue;

            foreach (BlockPlugin plugin in blockBehaviour.GetPlugins())
            {
                MoveablePlugin moveable = plugin as MoveablePlugin;
                if (moveable == null) continue;

                if (!moveable.IsDisplaced() ||
                    blockFaceOfNeighbouringBlock == moveable.GetDisplacedFace())
                {
                    BlockFace moveableFace = moveable.IsDisplaced()
                        ? moveable.GetDisplacedFace()
                        : blockFaceOfNeighbouringBlock;
                    blockFaceBehaviour.HighlightFace(blockFaceOfNeighbouringBlock);
                    _faceMap.Add(blockFaceBehaviour, moveableFace); ;
                }

                break;
            }
        }
    }

    GameObject GetGameObjInDir(Vector3 dir)
    {
        RaycastHit hit;

        Vector3 GridSpaceCoordinate = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z));

        //Debug.Log(GridSpaceCoordinate);

        Ray ray = new Ray(GridSpaceCoordinate, dir);
        if (Physics.Raycast(ray, out hit, 0.6f, CollidableLayer))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}
