using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public LayerMask CollidableLayers;
    public List<BlockPlugin> Plugins = new List<BlockPlugin>();
    public GameObject Root;

    [Range(0f, 1f)]
    public float SkinToLengthRatio = 0.1f;
    public float InitialSpeed = 1f;
    public ITransparentRenderer TransparentRenderer;

    private Vector3 _targetPosition;
    private float _currentSpeed;
    private float _acceleration;

    private bool _isTranslating;
    private bool _isPlayerStandingOn;
    private BlockFace _lastClickedFace;
    private BlockFaceBehaviour _blockFaceBehaviour;
    private List<BlockPlugin> _plugins = new List<BlockPlugin>();

    // Use this for initialization
    private void Start()
    {
        TransparentRenderer = GetComponent<ITransparentRenderer>();
        _blockFaceBehaviour = GetComponent<BlockFaceBehaviour>();
        _currentSpeed = InitialSpeed;

        Root = Root == null ? gameObject : Root;

        foreach (BlockPlugin plugin in Plugins)
        {
            BlockPlugin pluginInstance = Instantiate(plugin);
            SubscribePlugin(pluginInstance);
            _plugins.Add(pluginInstance);
        }
    }

    private void OnDestroy()
    {
        foreach (BlockPlugin plugin in _plugins)
        {
            Destroy(plugin);
        }

        transform.DetachChildren();
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

    public bool MoveBlock(BlockFace face, float initialSpeed = 1, float acceleration = 0)
    {
        if (_isTranslating)
        {
            return false;
        }

        _acceleration = acceleration;
        _currentSpeed = initialSpeed;

        bool hit = _blockFaceBehaviour.FireRaycastFromFace(SkinToLengthRatio, CollidableLayers, face);
        if (!hit)
        {
            _targetPosition = Root.transform.position + (face.GetNormal() * _blockFaceBehaviour.GetFaceLength());
            _isTranslating = true;
            _lastClickedFace = face;
            return true;
        }

        return false;
    }

    private void TranslateBlock()
    {
        _currentSpeed += _acceleration * Time.deltaTime;

        Vector3 translateDir = _targetPosition - Root.transform.position;
        Vector3 translate = translateDir.normalized * Time.deltaTime * _currentSpeed;

        if (Vector3.Distance(Root.transform.position, _targetPosition) > translate.magnitude)
        {
            Root.transform.Translate(translate);
        }
        else
        {
            Root.transform.position = _targetPosition;
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

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }

    public void SetCurrentSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
    }

    public List<BlockPlugin> GetPlugins()
    {
        return _plugins;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

    public GameObject GetCollidableObject(BlockFace face)
    {
        return _blockFaceBehaviour.GetRaycastObjectRef(SkinToLengthRatio, CollidableLayers, face);
    }
}
