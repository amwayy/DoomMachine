using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    [SerializeField] private Button endModifyButton;
    [SerializeField] private Transform usedDicesArea;
    [SerializeField] private List<Transform> utilitySlotsList;

    public static Utility Instance { get; private set; }

    private List<Dice> utilityDicesList;

    private void Awake()
    {
        Instance = this;

        utilityDicesList = new List<Dice>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerEndTurnButton += Player_OnPlayerEndTurnButton;
        StartUI.Instance.OnStart += StartUI_OnStart;
    }

    private void StartUI_OnStart(object sender, System.EventArgs e)
    {
        if (StartUI.Instance.IsHard())
        {
            utilitySlotsList[2].gameObject.SetActive(false);
            utilitySlotsList[3].gameObject.SetActive(false);
        }
    }

    private void Player_OnPlayerEndTurnButton(object sender, System.EventArgs e)
    {
        int utilityDicesNum = utilityDicesList.Count;
        if (utilityDicesNum != 0)
        {
            foreach(Dice utilityDice in utilityDicesList)
            {
                utilityDice.transform.SetParent(usedDicesArea);
            }
            utilityDicesList.Clear();
            endModifyButton.gameObject.SetActive(false);
        }
    }

    public void AddUtilityDices(Dice dice)
    {
        utilityDicesList.Add(dice);
    }

    public List<Dice> GetUtilityDicesList()
    {
        return utilityDicesList;
    }

    public bool HasUtilityDice()
    {
        return utilityDicesList.Count != 0;
    }
}
