using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public LayerMask CollidableLayers;
    public List<BlockPlugin> Plugins = new List<BlockPlugin>();

    [Range(0f, 1f)]
    public float SkinToLengthRatio = 0.1f;
    public float Speed = 1f;

    private Vector3 _targetPosition;

    private bool _isTranslating;
    private BlockFace _lastClickedFace;
    private BlockFaceBehaviour _blockFaceBehaviour;


    // Use this for initialization
    private void Start()
    {
        _blockFaceBehaviour = GetComponent<BlockFaceBehaviour>();
        foreach (BlockPlugin plugin in Plugins)
        {
            BlockPlugin pluginInstance = Instantiate(plugin);
            SubscribePlugin(pluginInstance);
        }
    }

    private void SubscribePlugin(BlockPlugin plugin)
    {
        plugin.Plug(this);

        //subscribe to events
        _onFaceClick += plugin.OnFaceClick;
        _onUpdate += plugin.OnUpdate;
    }

    public void UnsubscribePlugin(BlockPlugin plugin)
    {
        //unsubscribe to events
        _onFaceClick -= plugin.OnFaceClick;
        _onUpdate -= plugin.OnUpdate;
    }

    /**
     *  OnFaceClick(BlockFaceBehaviour)
     *  Triggers when the face of a block is clicked and block is interact-able
     */
    private delegate void OnFaceClickDel(BlockFace face);
    private OnFaceClickDel _onFaceClick;

    // TODO: Hide with Interface
    public void OnFaceClick(BlockFace face)
    {
        if (_onFaceClick != null && !_isTranslating)
        {
            _onFaceClick(face);
        }
    }

    private delegate void OnUpdateDel();
    private OnUpdateDel _onUpdate;

    private void Update()
    {
        if (_isTranslating)
        {
            TranslateBlock();
        }

        if (_onUpdate != null)
        {
            _onUpdate();
        }
    }

    public bool MoveBlock(BlockFace face)
    {
        if (_isTranslating)
        {
            return false;
        }

        bool hit = _blockFaceBehaviour.FireRaycastFromFace(SkinToLengthRatio, CollidableLayers, face);
        if (!hit)
        {
            _targetPosition = transform.position + (face.GetNormal() * _blockFaceBehaviour.GetFaceLength());
            _isTranslating = true;
            _lastClickedFace = face;
            return true;
        }

        return false;
    }

    private void TranslateBlock()
    {
        Vector3 translateDir = _targetPosition - transform.position;
        transform.Translate(translateDir.normalized * Time.deltaTime * Speed);
        if (Vector3.Distance(transform.position, _targetPosition) <= 0.01f)
        {
            transform.position = _targetPosition;
            _isTranslating = false;
        }
    }
}
