using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynapticDisruptor : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int damage;
    private int moveNum = 3;
    private int currentSlotIndex;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Sentience,
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
            if (enemy.TryGetComponent(out SynapticDisruptor synapticDisruptor))
            {
                aliveEnemyList.Remove(synapticDisruptor);
                break;
            }
        }
        base.EnemyDied();
    }

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 2 && (selectedDicesList[0].GetPip() == 4 && selectedDicesList[1].GetPip() == 5)
            || (selectedDicesList[0].GetPip() == 5 && selectedDicesList[1].GetPip() == 4);
    }

    public override void HandleMove()
    {
        base.HandleMove();

        damage = Sentience.Instance.GetSentience();
        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Sentience;
            Sentience.Instance.ModifySentience(1);
        }
        else if (currentMove == Move.Sentience)
        {
            currentMove = Move.Damage;
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage)
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
