using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlugin : ScriptableObject
{
    protected BlockBehaviour _block;

    private void OnDisable()
    {
        UnPlug();
    }

    public virtual void Plug(BlockBehaviour block)
    {
        _block = block;
    }

    private void UnPlug()
    {
        if (_block != null)
        {
            _block.UnsubscribePlugin(this);
        }
    }

    public virtual void OnFaceClick(BlockFace face) { }

    public virtual void OnFaceSelect(BlockFace face) { }

    public virtual Vector3 GetMoveDirection(BlockFace face)
    {
        // use Vector3.zero as error value
        return Vector3.zero;
    }

    public virtual void OnUpdate() { }
}
