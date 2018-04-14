using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public List<string> Scenes = new List<string>();
    public string SceneName;
    private List<string> _toLoad = new List<string>();

    private Vector3 _position;
    private bool _isLoadedToPosition = true;
    private GameObject _player;
    private bool _isReloading = false;
    
    private int _sceneIndex = 0;
    public CollectibleScore _collectibleScore;

    // we use Singleton Pattern
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Load("Player");
            _toLoad = Scenes;

        }
        else
        {
            //register scenes
            if (Scenes.Count > 0)
            {
                Instance.RegisterScenes(Scenes, SceneName);
            }
        }

        if (Scenes.Count > 0)
        {
            Load(Scenes[0]);
        }
    }
    
    public void RegisterScenes(List<string> scenes, string sceneName)
    {
        int index = _toLoad.IndexOf(sceneName);
        _toLoad.InsertRange(index, scenes);
        _toLoad.Remove(sceneName);

    }

    public void RegisterCheckpoint(Vector3 position)
    {
        Instance._registerCheckpoint(position);
    }

    private void _registerCheckpoint(Vector3 position)
    {
        _position = position;
        if (_isLoadedToPosition)
        {
            _isLoadedToPosition = false;
            _player = GameObject.FindGameObjectWithTag("Player");
            _player.transform.position = _position;
        }
    }

    public void RegisterCollectibleScore(CollectibleScore cs)
    {
        _collectibleScore = cs;
    }

    public void LoadNext()
    {
        Instance._loadNext();
    }

    private void _loadNext()
    {
        _collectibleScore.SaveScore();
        _sceneIndex++;
        if (_sceneIndex >= _toLoad.Count)
        {
            Debug.Log("No more scenes to load");
            return;
        }
        Load(_toLoad[_sceneIndex]);
    }

    public void UnloadPrevious()
    {
        Instance._UnloadPrevious();
    }

    private void _UnloadPrevious()
    {
        if (_sceneIndex == 0)
        {
            return;
        }
        Unload(_toLoad[_sceneIndex - 1]);
    }

    public void ReloadCurrentScene()
    {
        if (!_isReloading)
        {
            _isReloading = true;
            StartCoroutine(ReloadScene());
        }
    }

    public void LoadNewScene(String sceneName)
    {
        UnloadAllScenes();
        _isLoadedToPosition = true;
        _toLoad = Scenes;
        _toLoad.Add(sceneName);
        Load("Player");
        Camera.main.GetComponent<CameraController>().ResetCameraAngle();
        int index = _toLoad.IndexOf(sceneName);
        _toLoad = _toLoad.GetRange(index, _toLoad.Count - index);
        Load(_toLoad[0]);
        _sceneIndex = 0;
        _collectibleScore.ResetScore();
        Menu.Instance.UpdateUIScores();
    }

    void UnloadAllScenes()
    {
        Scene currentScene = gameObject.scene;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene sceneAtI = SceneManager.GetSceneAt(i);
            if (sceneAtI == currentScene) continue;
            if (!sceneAtI.isLoaded) continue;
            if (sceneAtI.name == "Player") continue;
            SceneManager.UnloadSceneAsync(sceneAtI);
        }
    }

    IEnumerator ReloadScene()
    {
        if (Scenes.Count == 0)
        {
            Debug.Log("Sorry reloading does not work for puzzles");
            yield break;
        }

        string sceneName = _toLoad[_sceneIndex];
        yield return SceneManager.UnloadSceneAsync(sceneName);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _collectibleScore.ResetScore();
        Camera.main.GetComponent<CameraController>().ResetCameraAngle();
        _player = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.GetRigidbody().velocity.Set(0, 0, 0);
        _player.transform.position = _position;
        playerController.SetMobility(true);
        _isReloading = false;
        _collectibleScore.ResetScore();
        Menu.Instance.UpdateUIScores();
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

    public string GetCurrentSceneName()
    {
        return _toLoad[_sceneIndex];
    }
}
