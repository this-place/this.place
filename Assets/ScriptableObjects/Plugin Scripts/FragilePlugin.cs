using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FragilePlugin : BlockPlugin
{
    public float BreakAfterTime;

    public override void OnUpdate()
    {
        if (_block.IsTranslating() || _block.IsPlayerStandingOn())
        {
            Destroy(_block.gameObject, BreakAfterTime);
        }
    }
}

