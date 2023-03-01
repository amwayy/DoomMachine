using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtinctionCollider : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int damage;
    private int moveNum = 5;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Damage1,
        Power1,
        Damage2,
        Power2
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
            if (enemy.TryGetComponent(out ExtinctionCollider extinctionCollider))
            {
                aliveEnemyList.Remove(extinctionCollider);
                break;
            }
        }
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 1 && (selectedDicesList[0].GetPip() == 4 || selectedDicesList[0].GetPip() == 5);
    }

    public override void HandleMove()
    {
        base.HandleMove();

        damage = Power.Instance.GetPower();
        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Damage1;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage1)
        {
            currentMove = Move.Power1;
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.Power1)
        {
            currentMove = Move.Damage2;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage2)
        {
            currentMove = Move.Power2;
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.Power2)
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
