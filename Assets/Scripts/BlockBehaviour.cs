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
    public List<BlockPlugin> Plugins = new List<BlockPlugin>();


    [Range(0f, 1f)]
    public float SkinToLengthRatio = 0.1f;
    public float Speed = 1f;

    private Vector3 _targetPosition;

    private bool _isTranslating;
    private BlockFaceBehaviour _lastClickedFace;

    // Use this for initialization
    private void Start()
    {
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
    }

    public void UnsubscribePlugin(BlockPlugin plugin)
    {
        //unsubscribe to events
        _onFaceClick -= plugin.OnFaceClick;
    }

    /**
     *  OnFaceClick(BlockFaceBehaviour)
     *  Triggers when the face of a block is clicked and block is interact-able
     */
    private delegate void OnFaceClickDel(BlockFaceBehaviour face);
    private OnFaceClickDel _onFaceClick;

    // TODO: Hide with Interface
    public void OnFaceClick(BlockFaceBehaviour face)
    {
        if (_onFaceClick != null && !_isTranslating)
        {
            _onFaceClick(face);
        }
    }

    private void Update()
    {
        if (_isTranslating)
        {
            TranslateBlock();
        }
    }

    public BlockFaceBehaviour GetOppositeFace(BlockFaceBehaviour face)
    {
        return
            (face == Top) ? Bottom :
            (face == Bottom) ? Top :
            (face == Front) ? Back :
            (face == Back) ? Front :
            (face == Left) ? Right :
            (face == Right) ? Left : null;
    }

    public bool MoveBlock(BlockFaceBehaviour face)
    {
        if (_isTranslating)
        {
            return false;
        }

        bool hit = face.FireRaycastFromFace(SkinToLengthRatio, CollidableLayers);
        if (!hit)
        {
            _targetPosition = transform.position + (face.GetNormal() * face.GetFaceLength());
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
