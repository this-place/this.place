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
        if (Physics.Raycast(ray, out hit))
        {
            // https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
            if (CollidableLayer == (CollidableLayer | (1 << hit.transform.gameObject.layer)))
            {
                BlockFaceBehaviour blockFace = hit.transform.gameObject.GetComponent<BlockFaceBehaviour>();
                BlockFace face = BlockFaceMethods.BlockFaceFromNormal(hit.normal);
                if (Input.GetMouseButtonDown(0))
                {
                    blockFace.OnMouseClick(face);
                }
                else
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
            else
            {
                LeaveLastBlock();
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
}
