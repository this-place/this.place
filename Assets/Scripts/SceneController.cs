using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private string _level = "Tutorial";
    private Vector3 _position;
    private GameObject _player;
    private PlayerController _playerController;
    private bool _isReloading = false;
    private Canvas _canvas;
    private CameraController _camera;
    private bool _menuShowing = true;
    private GameObject _sceneMenu;
    private GameObject _mainMenu;
    private GameObject _exitButtom;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        Load("Tutorial");
        Load("Player");
        HideMenu(_sceneMenu = GameObject.Find("SceneMenu"));
        _mainMenu = GameObject.Find("MainMenu");
        _exitButtom = GameObject.Find("ExitButton");
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _position = _player.transform.position;
        (_playerController = _player.GetComponent<PlayerController>()).SetMobility(false);
        _camera = Camera.main.GetComponent<CameraController>();
        _camera.SetIdle(true);
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
        if (Input.GetAxisRaw("Reload") == 1f && !_isReloading && !_menuShowing)
        {
            _isReloading = true;
            StartCoroutine(ReloadScene());
        }

        if (Input.GetAxisRaw("Pause") == 1f && !_menuShowing)
        {
            StopGame();
            ShowMenu(_mainMenu);
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

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        _camera.SetIdle(false);
        _playerController.SetMobility(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _exitButtom.SetActive(false);
        _menuShowing = false;
    }

    public void StopGame()
    {
        _camera.SetIdle(true);
        _playerController.SetMobility(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _exitButtom.SetActive(true);
        _menuShowing = true;
    }

    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void SwapMenu(GameObject menuToAdd, GameObject menuToRemove)
    {
        menuToAdd.SetActive(true);
        menuToRemove.SetActive(false);
    }
}
