using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public float FaceLength = 1;
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
    private UVMap _uvMap;

    private List<BlockPlugin> _plugins = new List<BlockPlugin>();

    private bool _isError = false;
    private Vector3 _originalPosition;

    public AudioSource DisplacementSound;
    public AudioSource DestroyedSound;

    public GameObject SelfDestructEmitter;

    private void Awake()
    {
        TransparentRenderer = GetComponent<ITransparentRenderer>();
        _uvMap = GetComponent<UVMap>();
        _currentSpeed = InitialSpeed;

        Root = Root == null ? gameObject : Root;
    }

    // Use this for initialization
    private void Start()
    {
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

        PlayerAnimatorController playerAnimControl = GetComponentInChildren<PlayerAnimatorController>();
        if (playerAnimControl != null)
        {
            playerAnimControl.UnParentPlayer();
        }
    }

    private void SubscribePlugin(BlockPlugin plugin)
    {
        plugin.Plug(this);

        //subscribe to events
        _onFaceClick += plugin.OnFaceClick;
        _onUpdate += plugin.OnUpdate;
        _onFaceSelect += plugin.OnFaceSelect;
        _getMoveDirectionDel += plugin.GetMoveDirection;

    }

    public void UnsubscribePlugin(BlockPlugin plugin)
    {
        //unsubscribe to events
        _onFaceClick -= plugin.OnFaceClick;
        _onUpdate -= plugin.OnUpdate;
        _onFaceSelect -= plugin.OnFaceSelect;
        _getMoveDirectionDel -= plugin.GetMoveDirection;
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

    private delegate void OnFaceSelectDel(BlockFace face);
    private OnFaceSelectDel _onFaceSelect;

    public void OnFaceSelect(BlockFace face)
    {
        if (_onFaceSelect != null && !_isTranslating)
        {
            _onFaceSelect(face);
        }
    }

    private delegate Vector3 GetMoveDirectionDel(BlockFace face);
    private GetMoveDirectionDel _getMoveDirectionDel;

    public Vector3 GetMoveDirection(BlockFace face)
    {
        if (_getMoveDirectionDel != null && !_isTranslating)
        {
            foreach (GetMoveDirectionDel del in _getMoveDirectionDel.GetInvocationList())
            {
                Vector3 direction = del(face);

                // use Vector3.zero as error value
                if (direction != Vector3.zero)
                {
                    return direction;
                }
            }
        }

        // use Vector3.zero as error value
        return Vector3.zero;
    }


    public bool MoveBlock(BlockFace face, float initialSpeed = 1, float acceleration = 0, bool isError = false)
    {
        if (_isTranslating)
        {
            return false;
        }

        _acceleration = acceleration;
        _currentSpeed = initialSpeed;

        bool hit = FireRaycastFromFace(face);
        if (!hit)
        {
            _isTranslating = true;


            _isError = isError;

            _targetPosition = _isError ? Root.transform.position + (face.GetNormal() * FaceLength / 2) :
                Root.transform.position + (face.GetNormal() * FaceLength);

            if (_isError)
            {
                _originalPosition = transform.position;
            }

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

            if (_isError)
            {
                _targetPosition = _originalPosition;
                _isError = false;
            }
            else
            {
                _isTranslating = false;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Vector3.Distance(collision.transform.position, transform.position) < 0.5f)
            {
                SceneController.Instance.ReloadCurrentScene();
            }
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
        GameObject SelfDestructEmitterInstantiated = Instantiate(SelfDestructEmitter, transform.position, transform.rotation) as GameObject;
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(SelfDestructEmitterInstantiated.gameObject, gameObject.scene);
        Destroy(SelfDestructEmitterInstantiated, 1.15f);
        Destroy(gameObject);
    }

    public GameObject GetCollidableObject(BlockFace face)
    {
        Vector3 normal = face.GetNormal();
        Vector3[] skinVertices = GetSkinVertices(normal, face);

        foreach (Vector3 skinVertex in skinVertices)
        {
            RaycastHit hit;

            if (Physics.Raycast(skinVertex, normal, out hit))
            {
                return hit.collider.gameObject;
            }

        }
        return null;
    }

    // Face Methods
    public Vector3[] GetSkinVertices(Vector3 normal, BlockFace face)
    {
        float centerToSkin = FaceLength * (1 - SkinToLengthRatio) / 2;
        Vector3[] perpendicularNormals = face.GetPerpendicularNormals();
        Vector3 quadCenter = transform.position + (centerToSkin * normal);

        Vector3[] skinVertices = new Vector3[4];
        Vector3 scaledPNormal1 = centerToSkin * perpendicularNormals[0];
        Vector3 scaledPNormal2 = centerToSkin * perpendicularNormals[1];

        skinVertices[0] = quadCenter + scaledPNormal1 + scaledPNormal2;
        skinVertices[1] = quadCenter + scaledPNormal1 - scaledPNormal2;
        skinVertices[2] = quadCenter - scaledPNormal1 + scaledPNormal2;
        skinVertices[3] = quadCenter - scaledPNormal1 - scaledPNormal2;

        return skinVertices;
    }

    public bool FireRaycastFromFace(BlockFace face, int faceLengths = 1)
    {
        Vector3 normal = face.GetNormal();
        Vector3[] skinVertices = GetSkinVertices(normal, face);

        foreach (Vector3 skinVertex in skinVertices)
        {
            Debug.DrawRay(skinVertex, normal * FaceLength * faceLengths, Color.red, 1f);
            if (Physics.Raycast(skinVertex, normal, FaceLength * faceLengths, CollidableLayers))
            {
                return true;
            }

        }
        return false;
    }

    public void HighlightFace(BlockFace face)
    {
        if (_uvMap != null)
            _uvMap.SetFaceHighlight(face);
    }

    public void UnhighlightFace()
    {
        if (_uvMap != null)
            _uvMap.SetNormalTexture();
    }

    public void PlayDisplacementSound()
    {
        if (DisplacementSound.clip != null)
        {
            AudioSource.PlayClipAtPoint(DisplacementSound.clip, transform.position);
        }
    }

    public void PlayDestroyedSound()
    {
        if (DisplacementSound.clip != null)
        {
            AudioSource.PlayClipAtPoint(DestroyedSound.clip, transform.position);
        }
    }
}
