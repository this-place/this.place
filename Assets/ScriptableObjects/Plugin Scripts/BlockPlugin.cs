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

    public void Plug(BlockBehaviour block)
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

    public virtual void OnUpdate() { }
}
