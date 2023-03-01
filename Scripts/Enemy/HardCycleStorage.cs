using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardCycleStorage : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    new public static HardCycleStorage Instance { get; private set; }

    private int moveNum = 4;
    private int damage = 4;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;
    private bool isToBoostAdjacent = false;

    private enum Move
    {
        Start,
        Boost1,
        Damage,
        Boost2
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
            if (enemy.TryGetComponent(out HardCycleStorage hardCycleStorage))
            {
                aliveEnemyList.Remove(hardCycleStorage);
                break;
            }
        }

        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 1 && (selectedDicesList[0].GetPip() > 3);
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Boost1;
            isToBoostAdjacent = true;
        }
        else if (currentMove == Move.Boost1)
        {
            currentMove = Move.Damage;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage)
        {
            currentMove = Move.Boost2;
            isToBoostAdjacent = true;
        }
        else if (currentMove == Move.Boost2)
        {
            currentMove = Move.Start;
        }
    }

    public void SetIsToBoostAdjacent(bool isToBoost)
    {
        isToBoostAdjacent = isToBoost;
    }

    public bool IsToBoostAdjacent()
    {
        return isToBoostAdjacent;
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
