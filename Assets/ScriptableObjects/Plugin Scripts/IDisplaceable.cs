using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplaceable
{

    bool DisplaceableInFaceDirection(BlockFace face);

    Vector3 GetDisplaceDirection(BlockFace face);

}
