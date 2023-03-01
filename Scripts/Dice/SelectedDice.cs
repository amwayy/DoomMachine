using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDice : MonoBehaviour
{
    [SerializeField] private UsedDices usedDicesArea;

    public event EventHandler OnDicesUsedUpByAttack;

    public static SelectedDice Instance { get; private set; }

    private List<Dice> selectedDiceList;

    private void Awake()
    {
        Instance = this;

        selectedDiceList = new List<Dice>();
    }

    private void Start()
    {
        Dice.OnAnyDiceSelected += Dice_OnDiceSelected;
        Dice.OnAnyDiceUnselected += Dice_OnDiceUnselected;
        Enemy.OnAnyTakeDamage += Enemy_OnTakeDamage;
    }

    private void Enemy_OnTakeDamage(object sender, System.EventArgs e)
    {
        foreach(Dice usedDice in selectedDiceList)
        {
            usedDice.SetSelectVisual(false);
            usedDice.transform.SetParent(usedDicesArea.transform);
            UsedDices.Instance.UpdateUsedDicesList();
        }
        if (DiceSpawner.Instance.GetAvailableDicesNum() == 0)
        {
            OnDicesUsedUpByAttack?.Invoke(this, EventArgs.Empty);
        }
        selectedDiceList.Clear();
    }

    private void Dice_OnDiceUnselected(object sender, Dice.OnDiceUnselectedEventArgs e)
    {
        selectedDiceList.Remove(e.dice);
    }

    private void Dice_OnDiceSelected(object sender, Dice.OnDiceSelectedEventArgs e)
    {
        selectedDiceList.Add(e.dice);
    }

    public List<Dice> GetSelectedDicesList()
    {
        return selectedDiceList;
    }
}
