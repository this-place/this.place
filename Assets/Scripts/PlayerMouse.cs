using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    public LayerMask CollidableLayer;

    private BlockFaceBehaviour _lastBlock;
    private BlockFace _lastFace;
    void Update()
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
            if (Input.GetMouseButtonDown(0))
            {
                blockFace.OnMouseClick(face);
            }
            else
            {
                ChangeFaceHighlight(blockFace, face);
            }
        }
        else
        {
            LeaveLastBlock();
        }
    }

    void LeaveLastBlock()
    {
        if (_lastBlock != null)
        {
            _lastBlock.OnMouseLeave();
            _lastBlock = null;
        }
    }

    void ChangeFaceHighlight(BlockFaceBehaviour blockFace, BlockFace face)
    {
        if (_lastBlock != blockFace || _lastFace != face)
        {
            if (_lastBlock != null)
            {
                _lastBlock.OnMouseLeave();
            }
            _lastBlock = blockFace;
            _lastFace = face;
            blockFace.OnMouseHover(face);
        }
    }
}
