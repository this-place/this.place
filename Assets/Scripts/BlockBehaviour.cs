using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    /*
     * Faces
     */
    public BlockFaceBehaviour Top;
    public BlockFaceBehaviour Bottom;
    public BlockFaceBehaviour Front;
    public BlockFaceBehaviour Back;
    public BlockFaceBehaviour Left;
    public BlockFaceBehaviour Right;

    public LayerMask CollidableLayers;

    [Range(0f, 1f)]
    public float SkinToLengthRatio = 0.1f;
    public float Speed = 1f;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

    private bool _isDisplaced;
    private bool _isTranslating;
    private BlockFaceBehaviour _lastClickedFace;

    // Use this for initialization
    private void Start()
    {
        _originalPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isTranslating)
        {
           TranslateBlock();
        }
    }

    private BlockFaceBehaviour GetOppositeFace(BlockFaceBehaviour face)
    {
        return
            (face == Top) ? Bottom :
            (face == Bottom) ? Top :
            (face == Front) ? Back :
            (face == Back) ? Front :
            (face == Left) ? Right :
            (face == Right) ? Left : null;
    }

    public void MoveBlock(BlockFaceBehaviour face)
    {
        if (_isTranslating)
        {
            return;
        }

        if (_isDisplaced)
        {
            BlockFaceBehaviour oppositeFace = GetOppositeFace(_lastClickedFace);
            bool hit = oppositeFace.FireRaycastFromFace(SkinToLengthRatio, CollidableLayers);
            if (!hit)
            {
                _targetPosition = _originalPosition;
                _isTranslating = true;
            }
        }
        else
        {
            bool hit = face.FireRaycastFromFace(SkinToLengthRatio, CollidableLayers);
            if (!hit)
            {
                _targetPosition = _originalPosition + (face.GetNormal() * face.GetFaceLength());
                _isTranslating = true;
                _lastClickedFace = face;
            }
        }
    }

    private void TranslateBlock()
    {
        Vector3 translateDir = _targetPosition - transform.position;
        transform.Translate(translateDir.normalized * Time.deltaTime * Speed);
        if (Vector3.Distance(transform.position, _targetPosition) <= 0.01f)
        {
            transform.position = _targetPosition;
            _isTranslating = false;

            _isDisplaced = _targetPosition != _originalPosition;
        }
    }

    
}
