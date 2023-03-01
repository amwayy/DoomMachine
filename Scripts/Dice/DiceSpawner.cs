using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawner : MonoBehaviour
{
    [SerializeField] private List<Dice> blackDicesList;
    [SerializeField] private Transform usedDicesArea;

    public static DiceSpawner Instance { get; private set; }

    private int allBlackDicesNum = 5;
    private int allYellowDicesNum = 0;

    private const int YELLOW_DICE_NUM_MAX = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartUI.Instance.OnStart += StartUI_OnStart;
        Player.Instance.OnPlayerEndTurnButton += Player_OnEndPlayerTurn;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        Enemy.OnAnyDiceLoot += Enemy_OnDiceLoot;
        UsedDices.Instance.OnDiceLocked += UsedDices_OnDiceLocked;
        UsedDices.Instance.OnDiceUnlocked += UsedDices_OnDiceUnlocked;
    }

    private void UsedDices_OnDiceUnlocked(object sender, UsedDices.OnDiceUnlockedEventArgs e)
    {
        if (e.unlockedDice.IsBlack())
        {
            allBlackDicesNum++;
        }
        if (e.unlockedDice.IsYellow())
        {
            allYellowDicesNum++;
        }
    }

    private void UsedDices_OnDiceLocked(object sender, UsedDices.OnDiceLockedEventArgs e)
    {
        if (e.lockedDice.IsBlack())
        {
            allBlackDicesNum--;
        }
        if (e.lockedDice.IsYellow())
        {
            allYellowDicesNum--;
        }
    }

    private void Enemy_OnDiceLoot(object sender, Enemy.OnDiceLootEventArgs e)
    {
        if(allYellowDicesNum < YELLOW_DICE_NUM_MAX)
        {
            allYellowDicesNum++;
        }
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, EventArgs e)
    {
        SpawnDices();
    }

    private void Player_OnEndPlayerTurn(object sender, EventArgs e)
    {
        List<Transform> unusedDicesList = new List<Transform>();
        foreach(Transform child in transform)
        {
            unusedDicesList.Add(child);
        }
        foreach(Transform unusedDiceTransform in unusedDicesList)
        {
            if(unusedDiceTransform.TryGetComponent(out Dice unusedDice) && unusedDice.IsSelected())
            {
                unusedDice.HandleSelect();
            }
            unusedDiceTransform.SetParent(usedDicesArea);
            UsedDices.Instance.UpdateUsedDicesList();
        }
    }

    private void StartUI_OnStart(object sender, EventArgs e)
    {
        SpawnDices();
    }

    public int GetAvailableDicesNum()
    {
        return transform.childCount;
    }

    private void SpawnDices()
    {
        int blackDicesNum = allBlackDicesNum;
        int yellowDicesNum = Math.Min(allYellowDicesNum, YELLOW_DICE_NUM_MAX);
        List<Dice> shieldDicesList = Shield.Instance.GetShieldDicesList();
        foreach(Dice shieldDice in shieldDicesList)
        {
            if (shieldDice.IsBlack())
            {
                blackDicesNum--;
            }
            if (shieldDice.IsYellow())
            {
                yellowDicesNum--;
            }
        }
        for(int i = 0; i < blackDicesNum; i++)
        {
            Instantiate(DiceManager.Instance.GetCertainPipBlackDice(UnityEngine.Random.Range(1, 7)), transform);
        }
        for (int i = 0; i < yellowDicesNum; i++)
        {
            Instantiate(DiceManager.Instance.GetCertainPipYellowDice(UnityEngine.Random.Range(1, 7)), transform);
        }
    }
}
