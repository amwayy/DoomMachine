using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfernalPowerComplex : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    new public static InfernalPowerComplex Instance { get; private set; }

    public int partsToDamageNum = 0;

    private int moveNum = 4;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        SentiencePower1,
        Idle,
        SentiencePower2
    }

    private void Awake()
    {
        Instance = this;

        currentSlotIndex = moveNum - 1;

        enemyButton.onClick.AddListener(() =>
        {
            TryTakeDamage();
        });

        InitiateHP();
    }

    public override void EnemyDied()
    {
        List<Enemy> aliveEnemyList = EnemyManager.Instance.GetAliveEnemiesList();
        foreach (Enemy enemy in aliveEnemyList)
        {
            if (enemy.TryGetComponent(out InfernalPowerComplex infernalPowerComplex))
            {
                aliveEnemyList.Remove(infernalPowerComplex);
                break;
            }
        }
        partsToDamageNum = aliveEnemyList.Count;
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 3 && selectedDicesList[0].GetPip() == 1 && selectedDicesList[1].GetPip() == 1 && selectedDicesList[2].GetPip() == 1;
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.SentiencePower1;
            ModifySentiencePower(1);
        }
        else if (currentMove == Move.SentiencePower1)
        {
            currentMove = Move.Idle;
        }
        else if (currentMove == Move.Idle)
        {
            currentMove = Move.SentiencePower2;
            ModifySentiencePower(1);
        }
        else if(currentMove == Move.SentiencePower2)
        {
            currentMove = Move.Start;
        }
    }

    private void HandleMoveVisual()
    {
        if (gameObject != null)
        {
            moveSelectedVisualList[currentSlotIndex].gameObject.SetActive(false);
            currentSlotIndex = (currentSlotIndex + 1) % moveNum;
            moveSelectedVisualList[currentSlotIndex].gameObject.SetActive(true);
        }
    }
}
