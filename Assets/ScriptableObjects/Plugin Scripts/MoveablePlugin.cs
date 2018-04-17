using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveablePlugin : BlockPlugin
{

    private bool _isDisplaced;
    private BlockFace _displacedFace;

    private void OnEnable()
    {
        _isDisplaced = false;
    }

    public override void OnFaceClick(BlockFace face)
    {
        if (_isDisplaced) return;

        bool isError = _block.FireRaycastFromFace(face, 2);

        if (_block.MoveBlock(face, isError: isError))
        {
            _isDisplaced = isError ? false : true;
            _displacedFace = face;
            _block.PlayDisplacementSound();
            if (!isError) // Only grey out block if it was displaced
            {
                _block.StartCoroutine(GreyOutBlock(_block.GetComponent<Renderer>().material));
            }
        }
    }

    public override void OnFaceSelect(BlockFace face)
    {
        if (_isDisplaced) return;
        _block.HighlightFace(face);
    }

    public override Vector3 GetMoveDirection(BlockFace face)
    {
        if (_isDisplaced) return Vector3.zero;
        return face.GetNormal();
    }

    IEnumerator GreyOutBlock(Material BlockMaterial)
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 3.0f;

        while (ElapsedTime < TotalTime)
        {
            BlockMaterial.color = Color.Lerp(BlockMaterial.color, _block.GreyedOutColor, ElapsedTime / TotalTime);

            ElapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
