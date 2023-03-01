using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private List<Dice> allBlackDicesList;
    [SerializeField] private List<Dice> allYellowDicesList;

    public static DiceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Dice GetCertainPipBlackDice(int pip)
    {
        foreach (Dice dice in allBlackDicesList)
        {
            if (dice.GetPip() == pip)
            {
                return dice;
            }
        }
        return null;
    }

    public Dice GetCertainPipYellowDice(int pip)
    {
        foreach (Dice dice in allYellowDicesList)
        {
            if (dice.GetPip() == pip)
            {
                return dice;
            }
        }
        return null;
    }

    //public List<Dice> GetAllDices()
    //{
    //    return allDicesList;
    //}
}
