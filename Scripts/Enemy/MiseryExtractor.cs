using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiseryExtractor : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    new public static MiseryExtractor Instance { get; private set; }

    public bool isDead = false;
    public int damage = 0;

    private int moveNum = 4;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Damage1,
        Damage2,
        Damage3
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
            if (enemy.TryGetComponent(out MiseryExtractor miseryExtractor))
            {
                aliveEnemyList.Remove(miseryExtractor);
                break;
            }
        }

        isDead = true;

        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 1 && (selectedDicesList[0].GetPip() == 2 || selectedDicesList[0].GetPip() == 3);
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
            currentMove = Move.Damage3;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage3)
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
