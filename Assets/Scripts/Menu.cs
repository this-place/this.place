using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private GameObject _player;
    private PlayerController _playerController;

    private Canvas _canvas;
    private CameraController _camera;
    private bool _menuShowing = true;
    private GameObject _sceneMenu;
    private GameObject _mainMenu;
    private GameObject _exitButtom;
    public Button _levelSelect;

    public Sprite[] levelTextures;
    public Image[] levelButtons;
    public Color defaultImageColor;
    public Color currentlyChosenImageColor = Color.black;
    private int currentLevelSelected = 0;
    public Text _levelName;

    // Use this for initialization
    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        defaultImageColor = levelButtons[0].color;
        HideMenu(_sceneMenu = GameObject.Find("SceneMenu"));
        _mainMenu = GameObject.Find("MainMenu");
        _exitButtom = GameObject.Find("ExitButton");
        levelButtons[currentLevelSelected].color = currentlyChosenImageColor;
        _levelName.text = levelTextures[currentLevelSelected].name.Split('.')[0];
        _levelSelect.image.sprite = levelTextures[currentLevelSelected];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Reload") == 1f && !_menuShowing)
        {
            SceneController.Instance.ReloadCurrentScene();
        }

        if (Input.GetAxisRaw("Pause") == 1f && !_menuShowing)
        {
            StopGame();
            ShowMenu(_mainMenu);
        }
    }

    void Start()
    {
        _camera = Camera.main.GetComponent<CameraController>();
        _camera.SetIdle(true);
        _player = GameObject.FindGameObjectWithTag("Player");
        (_playerController = _player.GetComponent<PlayerController>()).SetMobility(false);
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

    public void JumpToCurrentlySelectedLevel()
    {
        JumpToLevel(levelTextures[currentLevelSelected].name.Split('.')[0]);
    }

    public void JumpToLevel(string sceneName)
    {
        Camera.main.GetComponent<CameraController>().ResetCameraAngle();
        SceneController.Instance.LoadNewScene(sceneName);
    }

    public void changeLevelSelected(int value)
    {
        levelButtons[currentLevelSelected].color = defaultImageColor;
        currentLevelSelected += value;
        if (currentLevelSelected < 0)
            currentLevelSelected += levelTextures.Length;
        if (currentLevelSelected >= levelTextures.Length)
            currentLevelSelected -= levelTextures.Length;
        levelButtons[currentLevelSelected].color = currentlyChosenImageColor;
        _levelName.text = levelTextures[currentLevelSelected].name.Split('.')[0];
        _levelSelect.image.sprite = levelTextures[currentLevelSelected];
    }
}
