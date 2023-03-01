using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicMainframe : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int moveNum = 3;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Sentience1,
        Sentience2
    }

    private void Awake()
    {
        currentSlotIndex = moveNum - 1;

        enemyButton.onClick.AddListener(() =>
        {
            TryTakeDamage();
        });

        InitiateHP();
    }

    public override void EnemyDied()
    {
        Power.Instance.ModifyPower(2);

        List<Enemy> aliveEnemyList = EnemyManager.Instance.GetAliveEnemiesList();
        foreach (Enemy enemy in aliveEnemyList)
        {
            if (enemy.TryGetComponent(out LogicMainframe logicMainframe))
            {
                aliveEnemyList.Remove(logicMainframe);
                break;
            }
        }
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 2 && selectedDicesList[0].GetPip() == selectedDicesList[1].GetPip();
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Sentience1;
            Sentience.Instance.ModifySentience(1);
        }
        else if (currentMove == Move.Sentience1)
        {
            currentMove = Move.Sentience2;
            Sentience.Instance.ModifySentience(1);
        }
        else if (currentMove == Move.Sentience2)
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
