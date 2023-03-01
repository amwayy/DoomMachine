using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecyclingVats : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    new public static RecyclingVats Instance { get; private set; }

    private int moveNum = 4;
    private int damage = 4;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;
    private bool isToRepairAdjacent = false;

    private enum Move
    {
        Start,
        Damage1,
        Damage2,
        Repair,
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
            if (enemy.TryGetComponent(out RecyclingVats recyclingVats))
            {
                aliveEnemyList.Remove(recyclingVats);
                break;
            }
        }

        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 1 && (selectedDicesList[0].GetPip() == 1 || selectedDicesList[0].GetPip() == 6);
    }

    public override void HandleMove()
    {
        base.HandleMove();

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
            currentMove = Move.Repair;
            isToRepairAdjacent = true;
        }
        else if (currentMove == Move.Repair)
        {
            currentMove = Move.Start;
        }
    }

    public void SetIsToRepairAdjacent(bool isToRepair)
    {
        isToRepairAdjacent = isToRepair;
    }

    public bool IsToRepairAdjacent()
    {
        return isToRepairAdjacent;
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
