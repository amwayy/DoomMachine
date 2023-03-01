using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform pauseUI;
    [SerializeField] private Transform gameOverUI;

    public event EventHandler OnPlayerPhaseEnd;

    public static GameManager Instance { get; private set; }

    public enum State
    {
        GameReady,
        GamePlaying,
        GamePaused,
        GameOver
    }

    public enum Phase
    {
        Player,
        Enemy
    }

    private Phase phase;
    private State state;
    private bool isFirstRound = true;

    private void Awake()
    {
        Instance = this;

        phase = Phase.Player;

        state = State.GameReady;
    }

    private void Start()
    {
        Player.Instance.OnPlayerEndTurnButton += Player_OnPlayerEndTurnButton;
        StartUI.Instance.OnStart += StartUI_OnStart;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        SelectedDice.Instance.OnDicesUsedUpByAttack += SelectedDice_OnDicesUsedUpByAttack;
        // ShieldButton.OnDicesUsedUpByAnyShield += ShieldButton_OnDicesUsedUpByAnyShield;
        Enemy.OnDoomCoreDied += Enemy_OnDoomCoreDied;
        HitPoint.Instance.OnPlayerDied += HitPoint_OnPlayerDied;
        PauseUI.Instance.OnResume += PauseUI_OnResume;
        Shield.Instance.OnDicesUsedUpByShield += Shield_OnDicesUsedUpByShield;
        GameOverUI.Instance.OnRevive += GameOverUI_OnRevive;
    }

    private void GameOverUI_OnRevive(object sender, EventArgs e)
    {
        if (gameOverUI.gameObject.activeSelf)
        {
            state = State.GamePaused;
        }
        else
        {
            state = State.GamePlaying;
        }
    }

    private void Shield_OnDicesUsedUpByShield(object sender, EventArgs e)
    {
        if (!Utility.Instance.HasUtilityDice())
        {
            phase = Phase.Enemy;
            OnPlayerPhaseEnd?.Invoke(this, EventArgs.Empty);
            isFirstRound = false;
        }
    }

    private void PauseUI_OnResume(object sender, EventArgs e)
    {
        HandlePause();
    }

    private void HitPoint_OnPlayerDied(object sender, EventArgs e)
    {
        state = State.GameOver;
        gameOverUI.gameObject.SetActive(true);
        GameOverUI.Instance.HandleGameOver(false);
    }

    private void Enemy_OnDoomCoreDied(object sender, EventArgs e)
    {
        state = State.GameOver;
        gameOverUI.gameObject.SetActive(true);
        GameOverUI.Instance.HandleGameOver(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandlePause();
        }
    }

    public bool IsFirstRound()
    {
        return isFirstRound;
    }

    private void HandlePause()
    {
        if (state == State.GamePlaying)
        {
            state = State.GamePaused;
            pauseUI.gameObject.SetActive(true);
        }
        else if (state == State.GamePaused)
        {
            state = State.GamePlaying;
            pauseUI.gameObject.SetActive(false);
        }
    }

    //private void ShieldButton_OnDicesUsedUpByAnyShield(object sender, EventArgs e)
    //{
    //    if (!Utility.Instance.HasUtilityDice())
    //    {
    //        phase = Phase.Enemy;
    //        OnPlayerPhaseEnd?.Invoke(this, EventArgs.Empty);
    //        isFirstRound = false;
    //    }
    //}

    private void SelectedDice_OnDicesUsedUpByAttack(object sender, EventArgs e)
    {
        if (!Utility.Instance.HasUtilityDice())
        {
            phase = Phase.Enemy;
            OnPlayerPhaseEnd?.Invoke(this, EventArgs.Empty);
            isFirstRound = false;
        }
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, System.EventArgs e)
    {
        phase = Phase.Player;
    }

    private void StartUI_OnStart(object sender, System.EventArgs e)
    {
        state = State.GamePlaying;
    }

    private void Player_OnPlayerEndTurnButton(object sender, System.EventArgs e)
    {
        phase = Phase.Enemy;
        OnPlayerPhaseEnd?.Invoke(this, EventArgs.Empty);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsPlayerPhase()
    {
        return phase == Phase.Player;
    }

    public bool IsEnemyPhase()
    {
        return phase == Phase.Enemy;
    }
}
