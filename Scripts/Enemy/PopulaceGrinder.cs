using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulaceGrinder : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int damage = 4;
    private int repairNum = 2;
    private int moveNum = 3;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Damage,
        Repair
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
        List<Enemy> aliveEnemyList = EnemyManager.Instance.GetAliveEnemiesList();
        foreach (Enemy enemy in aliveEnemyList)
        {
            if (enemy.TryGetComponent(out PopulaceGrinder populaceGrinder))
            {
                aliveEnemyList.Remove(populaceGrinder);
                break;
            }
        }
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 2 && selectedDicesList[0].GetPip() == 3 && selectedDicesList[1].GetPip() == 3;
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if(currentMove == Move.Start)
        {
            currentMove = Move.Damage;
            MakeDamage(damage);
        }
        else if(currentMove == Move.Damage)
        {
            currentMove = Move.Repair;
            Repair(repairNum);
        }
        else if(currentMove == Move.Repair)
        {
            currentMove = Move.Start;
        }
    }

    private void HandleMoveVisual()
    {
        if(gameObject != null)
        {
            moveSelectedVisualList[currentSlotIndex].gameObject.SetActive(false);
            currentSlotIndex = (currentSlotIndex + 1) % moveNum;
            moveSelectedVisualList[currentSlotIndex].gameObject.SetActive(true);
        }
    }
}
