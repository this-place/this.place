using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Main Menu")]
    public Button LevelSelect;

    [Header("Scene Menu UI")]

    [Header("Stage Menu")]
    public Text StagePrimaryCollectibleNumber;
    public Text StageSecondaryCollectibleNumber;
    public Sprite[] LevelTextures;
    public GameObject[] StageNames;

    public Renderer SceneSecondaryCollectible;
    public List<Renderer> SceneSecondaryCollectibles = new List<Renderer>();
    [Header("Game")]
    public Text GamePrimaryCollectibleNumber;
    public Text GameSecondaryCollectibleNumber;
    
    public Color DefaultImageColor;
    public Color CurrentlyChosenImageColor = Color.black;

    public CollectibleScore[] CollectibleScores;
    public Image[] LevelButtons;

    private GameObject _player;
    private PlayerController _playerController;

    private CameraController _camera;

    [HideInInspector]
    public bool _menuShowing = true;
    private GameObject _sceneMenu;
    private GameObject _mainMenu;
    private GameObject _ui;
    private GameObject _exitButton;

    private int _currentLevelSelected = 0;

    // we use Singleton Pattern
    public static Menu Instance;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DefaultImageColor = LevelButtons[0].color;
        HideMenu(_sceneMenu = GameObject.Find("SceneMenu"));
        _mainMenu = GameObject.Find("MainMenu");
        _exitButton = GameObject.Find("ExitButton");
        foreach (GameObject stageName in StageNames)
        {
            stageName.SetActive(false);
        }
        StageNames[_currentLevelSelected].SetActive(true);
        HideMenu(_ui = GameObject.Find("UI"));
        LevelButtons[_currentLevelSelected].color = CurrentlyChosenImageColor;


        LevelSelect.image.sprite = LevelTextures[_currentLevelSelected];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIScores();
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

        foreach (CollectibleScore cs in CollectibleScores)
        {
            cs.WipeScore();
        }
        UpdateMenuScores();
    }

    public void UpdateMenuScores()
    {
        CollectibleScore currentSelectedScore = CollectibleScores[_currentLevelSelected];
        StagePrimaryCollectibleNumber.text = GetCollectedText(currentSelectedScore, true);
        StageSecondaryCollectibleNumber.text = GetCollectedText(currentSelectedScore, false);
    }

    public string GetCollectedText(CollectibleScore collectibleScore, bool primary)
    {
        List<bool> scores;
        if (primary)
        {
            scores = collectibleScore.GetPrimaryCollectedInStage();
        }
        else
        {
            scores = collectibleScore.GetSecondaryCollectedInStage();
        }

        int collectedNum = 0;
        foreach (bool collected in scores)
        {
            if (collected)
            {
                collectedNum++;
            }
        }

        return collectedNum + "/" + scores.Count;
    }

    public string GetRecordedText(CollectibleScore collectibleScore, bool primary)
    {
        List<bool> scores;
        if (primary)
        {
            scores = collectibleScore.GetPrimaryRecordInStage();
        }
        else
        {
            scores = collectibleScore.GetSecondaryRecordInStage();
        }

        int collectedNum = 0;
        foreach (bool collected in scores)
        {
            if (collected)
            {
                collectedNum++;
            }
        }
        return collectedNum + "/" + scores.Count;
    }

    public void UpdateUIScores()
    {
        CollectibleScore currentPlayingScoreSceneController = SceneController.Instance._collectibleScore;
        GamePrimaryCollectibleNumber.text = GetRecordedText(currentPlayingScoreSceneController, true);
        GameSecondaryCollectibleNumber.text = GetRecordedText(currentPlayingScoreSceneController, false);
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
        _exitButton.SetActive(false);
        _menuShowing = false;
        ShowMenu(_ui);
        UpdateUIScores();
    }

    public void StopGame()
    {
        _camera.SetIdle(true);
        _playerController.SetMobility(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _exitButton.SetActive(true);
        _menuShowing = true;
        HideMenu(_ui);
        UpdateMenuScores();
    }

    public void ShowMenu(GameObject menu)
    {
        if (menu == _sceneMenu)
        {
            ChangeLevelSelected(0);
        }
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
        JumpToLevel(LevelTextures[_currentLevelSelected].name.Split('.')[0]);
    }

    public void JumpToLevel(string sceneName)
    {
        Camera.main.GetComponent<CameraController>().ResetCameraAngle();
        SceneController.Instance._collectibleScore.ResetScore();
        SceneController.Instance.LoadNewScene(sceneName);
        UpdateUIScores();
    }

    public void ChangeLevelSelected(int value)
    {
        LevelButtons[_currentLevelSelected].color = DefaultImageColor;
        StageNames[_currentLevelSelected].SetActive(false);
        _currentLevelSelected += value;
        if (_currentLevelSelected < 0)
            _currentLevelSelected += LevelTextures.Length;
        if (_currentLevelSelected >= LevelTextures.Length)
            _currentLevelSelected -= LevelTextures.Length;
        LevelButtons[_currentLevelSelected].color = CurrentlyChosenImageColor;
        StageNames[_currentLevelSelected].SetActive(true);
        LevelSelect.image.sprite = LevelTextures[_currentLevelSelected];
        UpdateMenuScores();
    }
}
