using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public float Speed = 1f;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isDisplaced = false;
    private bool _isTranslating = false;

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

    public void MoveBlock(Vector3 dir, float faceLength)
    {
        if (_isTranslating)
        {
            return;
        }

        float halfFaceLength = faceLength / 2;

        if (_isDisplaced)
        {
            Vector3 dirToOriginal = _originalPosition - transform.position;

            //Fire Raycast to see if block can move to position
            bool hit = Physics.Raycast(transform.position, dirToOriginal.normalized, halfFaceLength * 2.75f); //some terrible hack
            if (!hit)
            {
                _targetPosition = _originalPosition;
                _isTranslating = true;
            }
        }
        else
        {
            bool hit = Physics.Raycast(_originalPosition, dir, halfFaceLength * 2.75f); //some terrible hack
            if (!hit)
            {
                _targetPosition = _originalPosition + (dir * faceLength);
                _isTranslating = true;
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
