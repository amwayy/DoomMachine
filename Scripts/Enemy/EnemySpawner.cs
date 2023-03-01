using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemyList;
    [SerializeField] private Enemy doomCore;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private TextMeshProUGUI unshownLeftEnemiesNumText;
    [SerializeField] private TextMeshProUGUI unshownRightEnemiesNumText;

    public event EventHandler<OnEnemySpawnEventArgs> OnEnemySpawn;
    public class OnEnemySpawnEventArgs : EventArgs
    {
        public Enemy spawnEnemy;
    }
    public event EventHandler OnMoveLeftButtonPressed;
    public event EventHandler OnMoveRightButtonPressed;

    public static EnemySpawner Instance { get; private set; }

    private bool isDoomCorePresent = false;
    private List<Enemy> aliveEnemiesList;
    private int shownFirstEnemyIndex = 1;
    private int selectedEnemyIndex = 1;
    private int unshownLeftEnemiesNum = 0;
    private int unshownRightEnemiesNum = 0;
    private int removedEnemiesNum = 6;

    private const int ENEMY_CARD_WIDTH = 460;
    private const int TOTAL_ENEMY_NUM = 16;

    private void Awake()
    {
        Instance = this;

        moveLeftButton.onClick.AddListener(() => { HandleMoveLeftButton(); });
        moveRightButton.onClick.AddListener(() => { HandleMoveRightButton(); });

        aliveEnemiesList = new List<Enemy>();
    }

    private void Start()
    {
        StartUI.Instance.OnStart += StartUI_OnStart;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        Enemy.OnEnemiesMoveLeft += Enemy_OnEnemiesMoveLeft;
        Enemy.OnEnemiesMoveRight += Enemy_OnEnemiesMoveRight;
        Enemy.OnAnyDiceLoot += Enemy_OnAnyDiceLoot;
    }

    private void Enemy_OnAnyDiceLoot(object sender, Enemy.OnDiceLootEventArgs e)
    {
        if(aliveEnemiesList.Count - shownFirstEnemyIndex < 2)
        {
            MoveLeft();
        }
        if(e.enemyIndex == 0)
        {
            MoveRight();
        }
    }

    private void Enemy_OnEnemiesMoveRight(object sender, EventArgs e)
    {
        MoveRight();
    }

    private void Enemy_OnEnemiesMoveLeft(object sender, EventArgs e)
    {
        MoveLeft();
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, EventArgs e)
    {
        if(aliveEnemiesList.Count < 10)
        {
            SpawnEnemy();
        }

        selectedEnemyIndex = shownFirstEnemyIndex;
    }

    private void StartUI_OnStart(object sender, System.EventArgs e)
    {
        removedEnemiesNum = TOTAL_ENEMY_NUM - StartUI.Instance.GetEnemyNum();

        // 去除部份敌人
        for (int i = 0; i < removedEnemiesNum; i++)
        {
            if(enemyList.Count > 3)
            {
                enemyList.RemoveAt(UnityEngine.Random.Range(0, enemyList.Count));
            }
        }

        SpawnInitialEnemies();
    }

    private void Update()
    {
        aliveEnemiesList = EnemyManager.Instance.GetAliveEnemiesList();

        unshownLeftEnemiesNum = shownFirstEnemyIndex - 1;
        unshownRightEnemiesNum = aliveEnemiesList.Count - shownFirstEnemyIndex - 2;
        unshownLeftEnemiesNumText.text = unshownLeftEnemiesNum.ToString();
        if(unshownRightEnemiesNum >= 0)
        {
            unshownRightEnemiesNumText.text = unshownRightEnemiesNum.ToString();
        }

        if (Input.GetKeyDown(KeyCode.A) && GameManager.Instance.IsGamePlaying())
        {
            HandleMoveLeftButton();
        }
        if (Input.GetKeyDown(KeyCode.D) && GameManager.Instance.IsGamePlaying())
        {
            HandleMoveRightButton();
        }
    }

    public int GetContainerFirstIndex()
    {
        return shownFirstEnemyIndex;
    }

    public int GetSelectedEnemyIndex()
    {
        return selectedEnemyIndex;
    }

    private void HandleMoveLeftButton()
    {
        OnMoveLeftButtonPressed?.Invoke(this, EventArgs.Empty);

        if(selectedEnemyIndex > 1)
        {
            selectedEnemyIndex--;
            if(selectedEnemyIndex < shownFirstEnemyIndex)
            {
                MoveLeft();
            }
        }
    }

    private void HandleMoveRightButton()
    {
        OnMoveRightButtonPressed?.Invoke(this, EventArgs.Empty);

        if (selectedEnemyIndex < aliveEnemiesList.Count)
        {
            selectedEnemyIndex++;
            if (selectedEnemyIndex > shownFirstEnemyIndex + 2)
            {
                MoveRight();
            }
        }
    }

    public void MoveLeft()
    {
        if(shownFirstEnemyIndex > 1)
        {
            shownFirstEnemyIndex--;
            transform.position -= Vector3.left * ENEMY_CARD_WIDTH;
        }
        // Debug.Log("Current Shown First Enemy Index: " + currentShownFirstEnemyIndex);
    }

    public void MoveRight()
    {
        if (shownFirstEnemyIndex + 2 < aliveEnemiesList.Count)
        {
            shownFirstEnemyIndex++;
            transform.position -= Vector3.right * ENEMY_CARD_WIDTH;
        }
        // Debug.Log("Current Shown First Enemy Index: " + currentShownFirstEnemyIndex);
    }

    private void SpawnInitialEnemies()
    {
        int initialEnemyNum = 3;
        for(int i = 0; i < initialEnemyNum; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if(enemyList.Count != 0)
        {
            Enemy spawnEnemy = enemyList[UnityEngine.Random.Range(0, enemyList.Count)];
            Instantiate(spawnEnemy, transform);
            enemyList.Remove(spawnEnemy);
            OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs { spawnEnemy = spawnEnemy });
        }
        else if (!isDoomCorePresent)
        {
            Instantiate(doomCore, transform);
            OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs { spawnEnemy = doomCore });
            isDoomCorePresent = true;
        }
    }
}
