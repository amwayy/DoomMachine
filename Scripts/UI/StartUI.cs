using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button difficultySelectLeftButton;
    [SerializeField] private Button difficultySelectRightButton;
    [SerializeField] private Button enemyNumModifyLeftButton;
    [SerializeField] private Button enemyNumModifyRightButton;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI enemyNumValueText;
    [SerializeField] private OptionsUI optionsUI;

    public event EventHandler OnStart;
    public event EventHandler OnStartSetting;

    public static StartUI Instance { get; private set; }

    private List<string> difficultyTextsList;
    private int difficultyIndex = 1;
    private int enemyNum = 10;
    private enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    private Difficulty difficulty = Difficulty.Medium;
    private List<Difficulty> difficultyList;

    private const int ENEMY_NUM_MIN = 10;
    private const int ENEMY_NUM_MAX = 16;
    private const string IS_FULL_SCREEEN = "IsFullScreen";

    private void Awake()
    {
        Instance = this;

        HandleFullScreen();

        difficultyTextsList = new List<string> { "¼òµ¥", "ÆÕÍ¨", "À§ÄÑ" };
        difficultyList = new List<Difficulty> { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };

        startButton.onClick.AddListener(() => 
        {
            OnStart?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        });

        difficultySelectLeftButton.onClick.AddListener(() => 
        {
            OnStartSetting?.Invoke(this, EventArgs.Empty);
            difficultyIndex = (difficultyIndex + 2) % 3;
            difficultyText.text = difficultyTextsList[difficultyIndex];
            difficulty = difficultyList[difficultyIndex];
        });

        difficultySelectRightButton.onClick.AddListener(() =>
        {
            OnStartSetting?.Invoke(this, EventArgs.Empty);
            difficultyIndex = (difficultyIndex + 1) % 3;
            difficultyText.text = difficultyTextsList[difficultyIndex];
            difficulty = difficultyList[difficultyIndex];
        });

        enemyNumModifyLeftButton.onClick.AddListener(() =>
        {
            OnStartSetting?.Invoke(this, EventArgs.Empty);
            if (enemyNum > ENEMY_NUM_MIN)
            {
                enemyNum--;
            }
            enemyNumValueText.text = enemyNum.ToString();
        });

        enemyNumModifyRightButton.onClick.AddListener(() =>
        {
            OnStartSetting?.Invoke(this, EventArgs.Empty);
            if (enemyNum < ENEMY_NUM_MAX)
            {
                enemyNum++;
            }
            enemyNumValueText.text = enemyNum.ToString();
        });
    }

    private void HandleFullScreen()
    {
        if (PlayerPrefs.HasKey(IS_FULL_SCREEEN))
        {
            if(PlayerPrefs.GetInt(IS_FULL_SCREEEN) == 1)
            {
                Screen.fullScreen = true;
                Debug.Log("FullScreen");
            }
            else
            {
                Screen.fullScreen = false;
                Debug.Log("Windowed");
            }
        }
    }

    public int GetEnemyNum()
    {
        return enemyNum;
    }

    public bool IsEasy()
    {
        return difficulty == Difficulty.Easy;
    }

    public bool IsMedium()
    {
        return difficulty == Difficulty.Medium;
    }

    public bool IsHard()
    {
        return difficulty == Difficulty.Hard;
    }
}
