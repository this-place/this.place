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
    public float InitialSpeed = 1f;
    public float TerminalSpeed = 10f;

    private float _currentSpeed;
    private const float Acceleration = 3f;

    private Vector3 _targetPosition;

    private bool _isTranslating;
    private bool _isPlayerStandingOn;
    private BlockFace _lastClickedFace;
    private BlockFaceBehaviour _blockFaceBehaviour;


    // Use this for initialization
    private void Start()
    {
        _blockFaceBehaviour = GetComponent<BlockFaceBehaviour>();
        ResetSpeed();

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

        ResetSpeed();
        
        return false;
    }

    private void TranslateBlock()
    {
        UpdateSpeed();

        Vector3 translateDir = _targetPosition - transform.position;
        Vector3 translate = translateDir.normalized * Time.deltaTime * _currentSpeed;

        if (Vector3.Distance(transform.position, _targetPosition) > translate.magnitude)
        {
            transform.Translate(translate);
        }
        else
        {
            transform.position = _targetPosition;
            _isTranslating = false;
        }
    }


    public void SetIsPlayerStandingOn(bool isPlayerStandingOn)
    {
        _isPlayerStandingOn = isPlayerStandingOn;
    }

    public bool IsPlayerStandingOn()
    {
        return _isPlayerStandingOn;
    }

    public bool IsTranslating()
    {
        return _isTranslating;
    }

    public void UpdateSpeed()
    {
        if (_currentSpeed < TerminalSpeed)
        {
            _currentSpeed += Time.deltaTime * Acceleration;
        }
    }

    public void ResetSpeed()
    {
        _currentSpeed = InitialSpeed;
    }
}
