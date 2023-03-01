using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulsePools : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int moveNum = 4;
    private int damage = 3;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Power1,
        Power2,
        Damage
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
            if (enemy.TryGetComponent(out PulsePools pulsePools))
            {
                aliveEnemyList.Remove(pulsePools);
                break;
            }
        }
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 2 && (selectedDicesList[0].GetPip() == 1 && selectedDicesList[1].GetPip() == 6)
            ||(selectedDicesList[0].GetPip() == 6 && selectedDicesList[1].GetPip() == 1);
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Power1;
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.Power1)
        {
            currentMove = Move.Power2;
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.Power2)
        {
            currentMove = Move.Damage;
            MakeDamage(damage);
        }
        else if(currentMove == Move.Damage)
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
