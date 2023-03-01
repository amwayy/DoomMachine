using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomCore : Enemy
{
    [SerializeField] private List<Transform> moveSelectedVisualList;
    [SerializeField] private Button enemyButton;

    private int moveNum = 4;
    private int currentSlotIndex;
    private int damage;
    private Move currentMove = Move.Start;

    private enum Move
    {
        Start,
        Damage,
        SentiencePower1,
        SentiencePower2
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

    public override bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return selectedDicesList.Count == 3 && selectedDicesList[0].GetPip() == 6 && selectedDicesList[1].GetPip() == 6 && selectedDicesList[2].GetPip() == 6;
    }

    public override void HandleMove()
    {
        base.HandleMove();

        HandleMoveVisual();
        if (currentMove == Move.Start)
        {
            currentMove = Move.Damage;
            damage = Sentience.Instance.GetSentience() + Power.Instance.GetPower();
            MakeDamage(damage);
        }
        else if (currentMove == Move.Damage)
        {
            currentMove = Move.SentiencePower1;
            Sentience.Instance.ModifySentience(1);
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.SentiencePower1)
        {
            currentMove = Move.SentiencePower2;
            Sentience.Instance.ModifySentience(1);
            Power.Instance.ModifyPower(1);
        }
        else if (currentMove == Move.SentiencePower2)
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
