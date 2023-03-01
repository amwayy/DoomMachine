using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reroll : MonoBehaviour
{
    public static event EventHandler OnAnyDiceReroll;

    public static void ResetStaticData()
    {
        OnAnyDiceReroll = null;
    }

    private Button rerollSlotButton;
    private bool hasDice = false;
    private bool canReroll = true;
    private Dice rerolledDice = null;
    private List<Dice> utilityDicesList;

    private void Awake()
    {
        rerollSlotButton = GetComponent<Button>();
        rerollSlotButton.onClick.AddListener(() => { HandleReroll(); });
    }

    private void Start()
    {
        EndModifyButton.Instance.OnEndModify += EndModifyButton_OnEndModify;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        Player.Instance.OnPlayerEndTurnButton += Player_OnPlayerEndTurnButton;

        utilityDicesList = Utility.Instance.GetUtilityDicesList();
    }

    private void Player_OnPlayerEndTurnButton(object sender, System.EventArgs e)
    {
        hasDice = false;
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, System.EventArgs e)
    {
        canReroll = true;
    }

    private void EndModifyButton_OnEndModify(object sender, System.EventArgs e)
    {
        if(rerolledDice != null)
        {
            rerolledDice.transform.SetParent(DiceSpawner.Instance.transform);
            rerolledDice = null;
        }
        canReroll = false;
        hasDice = false;
    }

    private void HandleReroll()
    {
        List<Dice> selectedDicesList = SelectedDice.Instance.GetSelectedDicesList();
        if (selectedDicesList.Count == 1 && !hasDice && canReroll)
        {
            OnAnyDiceReroll?.Invoke(this, EventArgs.Empty);

            // Movement
            Dice selectedDice = selectedDicesList[0];
            selectedDice.HandleSelect();
            selectedDice.transform.SetParent(transform);
            selectedDice.transform.position = transform.position;
            hasDice = true;
            utilityDicesList.Add(selectedDice);
            EndModifyButton.Instance.gameObject.SetActive(true);
            
            // Reroll
            Transform parentTransform = selectedDice.transform.parent;
            int randomPip = UnityEngine.Random.Range(1, 7);
            Dice replaceDice;
            if (selectedDice.IsBlack())
            {
                replaceDice = DiceManager.Instance.GetCertainPipBlackDice(randomPip);
            }
            else
            {
                replaceDice = DiceManager.Instance.GetCertainPipYellowDice(randomPip);
            }
            utilityDicesList.Remove(selectedDice);
            Destroy(selectedDice.gameObject);
            rerolledDice = Instantiate(replaceDice, parentTransform);
            rerolledDice.transform.position = parentTransform.position;
            utilityDicesList.Add(rerolledDice);
        }
    }
}
