using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public event EventHandler OnEnemyPhaseEnd;

    public static EnemyManager Instance { get; private set; }

    private List<Enemy> aliveEnemiesList;
    private float afterMoveWaitTimer;
    private float afterMoveWaitTimerMax = 1f;
    private int moveEndEnemieNum = 0;

    private void Awake()
    {
        Instance = this;

        aliveEnemiesList = new List<Enemy>();

        afterMoveWaitTimer = afterMoveWaitTimerMax;
    }

    private void Start()
    {
        EnemySpawner.Instance.OnEnemySpawn += EnemySpawner_OnEnemySpawn;
        Enemy.OnAnyEnemyMoveEnd += Enemy_OnMoveEnd;
    }

    private void Update()
    {
        if(GameManager.Instance.IsEnemyPhase() && moveEndEnemieNum == aliveEnemiesList.Count)
        {
            afterMoveWaitTimer -= Time.deltaTime;
            if(afterMoveWaitTimer < 0)
            {
                OnEnemyPhaseEnd?.Invoke(this, EventArgs.Empty);
                moveEndEnemieNum = 0;
                afterMoveWaitTimer = afterMoveWaitTimerMax;
            }
        }
    }

    private void Enemy_OnMoveEnd(object sender, EventArgs e)
    {
        moveEndEnemieNum++;
    }

    private void EnemySpawner_OnEnemySpawn(object sender, EnemySpawner.OnEnemySpawnEventArgs e)
    {
        aliveEnemiesList.Add(e.spawnEnemy);
    }

    public List<Enemy> GetAliveEnemiesList()
    {
        return aliveEnemiesList;
    }
}
