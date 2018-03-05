using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    public LayerMask CollidableLayer;

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

                if (Input.GetMouseButtonDown(0))
                {
                    blockFace.OnMouseClick(hit.normal);

                }
            }
        }
    }
}
