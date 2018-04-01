using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    private string _level = "Tutorial";
    private Vector3 _position;
    private GameObject _player;
    private bool _isReloading = false;
    private void Awake()
    {
        Load("Tutorial");
        Load("Player");
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _position = _player.transform.position;
    }

    void Load(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    void Unload(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_isReloading)
        {
            _isReloading = true;
            StartCoroutine(ReloadScene());
        }
    }

    IEnumerator ReloadScene()
    {
        yield return SceneManager.UnloadSceneAsync(_level);
        yield return SceneManager.UnloadSceneAsync("Player");
        yield return SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(_level, LoadSceneMode.Additive);
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.transform.position = _position;
        _isReloading = false;
    }

    public void SetPosition(Vector3 pos)
    {
        _position = pos;
    }

    public void SetLevel(string level)
    {
        _level = level;
    }
}
