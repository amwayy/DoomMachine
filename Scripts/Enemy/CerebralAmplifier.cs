using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CerebralAmplifier : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int damage = 3;
    private int moveNum = 3;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Damage1,
        Damage2
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
            if (enemy.TryGetComponent(out CerebralAmplifier cerebralAmplifier))
            {
                aliveEnemyList.Remove(cerebralAmplifier);
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

        if(Sentience.Instance.GetSentience() > 5)
        {
            damage = 6;
        }
        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Damage1;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage1)
        {
            currentMove = Move.Damage2;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage2)
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
