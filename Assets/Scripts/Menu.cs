using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Main Menu")]
    public Button LevelSelect;
    
    [Header("Scene Menu UI")]
    public Text LevelName;
    public Text ScenePrimaryCollectibleNumber;
    public Renderer SceneSecondaryCollectible;
    public List<Renderer> SceneSecondaryCollectibles = new List<Renderer>();
    private float _sceneSecondaryTranslateAmount;
    private float _sceneSecondaryOffset = 10;
    public Text LevelPrimaryCollectibleNumber;
    public Renderer LevelSecondaryCollectible;
    public Text GamePrimaryCollectibleNumber;
    public Renderer GameSecondaryCollectible;
    public List<Renderer> GameSecondaryCollectibles = new List<Renderer>();
    private float _gameSecondaryTranslateAmount;
    private float _gameSecondaryOffset = 5;
    public Color DefaultImageColor;
    public Color CurrentlyChosenImageColor = Color.black;
    public Sprite[] LevelTextures;
    public CollectibleScore[] CollectibleScores;
    public Image[] LevelButtons;

    private GameObject _player;
    private PlayerController _playerController;

    private CameraController _camera;
    private bool _menuShowing = true;
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
        HideMenu(_ui = GameObject.Find("UI"));
        LevelButtons[_currentLevelSelected].color = CurrentlyChosenImageColor;
        LevelName.text = LevelTextures[_currentLevelSelected].name.Split('.')[0];
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
        _gameSecondaryTranslateAmount = 0;
        _sceneSecondaryTranslateAmount = 0;
        _gameSecondaryOffset = (GameSecondaryCollectibles[1].transform.position.x - GameSecondaryCollectibles[0].transform.position.x)/2;
        _sceneSecondaryOffset = (SceneSecondaryCollectibles[1].transform.position.x - SceneSecondaryCollectibles[0].transform.position.x) / 2;
        foreach (CollectibleScore cs in CollectibleScores)
        {
            cs.WipeScore();
        }
        UpdateMenuScores();
        UpdateUIScores();
    }

    public void UpdateMenuScores()
    {
        int totalCollectibles = 0;
        int currentCollectibles = 0;
        foreach (CollectibleScore cs in CollectibleScores)
        {
            foreach (bool isCollected in cs.GetPrimaryCollectedInStage())
            {
                if (isCollected)
                {
                    currentCollectibles++;
                }
            }
            totalCollectibles += cs.GetPrimaryCollectedInStage().Count;
        }
        GamePrimaryCollectibleNumber.text = currentCollectibles.ToString() +
            "/" +
            totalCollectibles.ToString();

        currentCollectibles = 0;
        foreach (bool isCollected in CollectibleScores[_currentLevelSelected].GetPrimaryCollectedInStage())
        {
            if (isCollected)
            {
                currentCollectibles++;
            }
        }
        ScenePrimaryCollectibleNumber.text = currentCollectibles.ToString() +
            "/" +
            CollectibleScores[_currentLevelSelected].GetPrimaryCollectedInStage().Count.ToString();
        
        List<bool> collectibleScore = CollectibleScores[_currentLevelSelected].GetSecondaryCollectedInStage();
        for (int i = 0; i < collectibleScore.Count; i++)
        {
            SceneSecondaryCollectibles[i].enabled = true;
            if (collectibleScore[i]) //collected
            {
                SceneSecondaryCollectibles[i].material.color = Color.white;
            }
            else
            {
                SceneSecondaryCollectibles[i].material.color = Color.black;
            }
        }
        for (int i = collectibleScore.Count; i < SceneSecondaryCollectibles.Count; i++)
        {
            SceneSecondaryCollectibles[i].enabled = false;
        }
        float offset = _sceneSecondaryOffset * (SceneSecondaryCollectibles.Count - collectibleScore.Count);
        for (int i = 0; i < SceneSecondaryCollectibles.Count; i++)
        {
            SceneSecondaryCollectibles[i].GetComponent<CollectiblesScript>().ResetRotation();
            SceneSecondaryCollectibles[i].transform.Translate(Vector3.left * _sceneSecondaryTranslateAmount);
            SceneSecondaryCollectibles[i].transform.Translate(Vector3.right * offset);
        }
        _sceneSecondaryTranslateAmount = offset;

        CollectibleScore tempCS;
        int currentValue = 0;
        for (int j=0; j< CollectibleScores.Length; j++)
        {
            tempCS = CollectibleScores[j];
            collectibleScore = tempCS.GetSecondaryCollectedInStage();
            for (int i = 0; i < collectibleScore.Count; i++)
            {
                GameSecondaryCollectibles[currentValue].enabled = true;
                if (collectibleScore[i]) //collected
                {
                    GameSecondaryCollectibles[currentValue].material.color = Color.white;
                }
                else
                {
                    GameSecondaryCollectibles[currentValue].material.color = Color.black;
                }
                currentValue++;
            }
        }
        for (int i = currentValue; i < GameSecondaryCollectibles.Count; i++)
        {
            GameSecondaryCollectibles[i].enabled = false;
        }
        offset = _gameSecondaryOffset * (GameSecondaryCollectibles.Count - currentValue);
        for (int i=0; i< GameSecondaryCollectibles.Count; i++)
        {
            GameSecondaryCollectibles[i].transform.Translate(Vector3.left * _gameSecondaryTranslateAmount);
            //GameSecondaryCollectibles[i].transform.
            GameSecondaryCollectibles[i].transform.Translate(Vector3.right * offset);
        }
        _gameSecondaryTranslateAmount = offset;
        
    }

    public void UpdateUIScores()
    {
        int currentCollectibles = 0;
        foreach (bool isCollected in SceneController.Instance._collectibleScore.GetPrimaryRecordInStage())
        {
            if (isCollected)
            {
                currentCollectibles++;
            }
        }
        LevelPrimaryCollectibleNumber.text = currentCollectibles +
            "/" +
            SceneController.Instance._collectibleScore.GetPrimaryCollectedInStage().Count.ToString();

        int previousCollectibles = 0;
        currentCollectibles = 0;
        foreach (bool isCollected in SceneController.Instance._collectibleScore.GetSecondaryRecordInStage())
        {
            if (isCollected)
            {
                currentCollectibles++;
            }
        }
        foreach (bool isCollected in SceneController.Instance._collectibleScore.GetSecondaryCollectedInStage())
        {
            if (isCollected)
            {
                previousCollectibles++;
            }
        }
        if (SceneController.Instance._collectibleScore.GetSecondaryCollectedInStage().Count < 1)
        {
            LevelSecondaryCollectible.enabled = false;
        }
        else
        {
            LevelSecondaryCollectible.enabled = true;
            if (currentCollectibles < 1 && previousCollectibles < 1) //uncollected previously and currently
            {
                LevelSecondaryCollectible.material.color = Color.white;
            }
            else if (currentCollectibles > 0) //collected
            {
                LevelSecondaryCollectible.material.color = Color.blue;
            }
            else //uncollected but previously collected
            {
                LevelSecondaryCollectible.material.color = Color.black;
            }
        }
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
        _currentLevelSelected += value;
        if (_currentLevelSelected < 0)
            _currentLevelSelected += LevelTextures.Length;
        if (_currentLevelSelected >= LevelTextures.Length)
            _currentLevelSelected -= LevelTextures.Length;
        LevelButtons[_currentLevelSelected].color = CurrentlyChosenImageColor;
        LevelName.text = LevelTextures[_currentLevelSelected].name.Split('.')[0];
        LevelSelect.image.sprite = LevelTextures[_currentLevelSelected];
        UpdateMenuScores();
    }
}
