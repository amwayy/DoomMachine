using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modify : MonoBehaviour
{
    [SerializeField] private Button modifyPlusButton;
    [SerializeField] private Button modifyMinusButton;

    public static event EventHandler OnAnyDiceModified;

    public static void ResetStaticData()
    {
        OnAnyDiceModified = null;
    }

    private Button modifySlotButton;
    private Dice modifyDice;
    private Dice modifiedDice = null;
    private bool hasDice = false;
    private bool canModify = true;
    private int currentPip;
    private List<Dice> utilityDicesList;

    private void Awake()
    {
        modifySlotButton = GetComponent<Button>();
        modifySlotButton.onClick.AddListener(() => 
        {
            if (canModify)
            {
                HandleMovement();
            }
        });

        HideButtons();

        modifyPlusButton.onClick.AddListener(() => 
        {
            if (currentPip < 6) {
                HandleModify(+1);
            }
        }); 
        modifyMinusButton.onClick.AddListener(() => 
        {
            if (currentPip > 1)
            {
                HandleModify(-1);
            }
        });
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
        HideButtons();
        hasDice = false;
        EndModifyButton.Instance.unmodifiedDicesNum = 0;
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, System.EventArgs e)
    {
        canModify = true;
    }

    private void EndModifyButton_OnEndModify(object sender, System.EventArgs e)
    {
        if(modifiedDice != null)
        {
            modifiedDice.transform.SetParent(DiceSpawner.Instance.transform);
            modifiedDice = null;
        }
        canModify = false;
        hasDice = false;
    }

    private void HandleMovement()
    {
        List<Dice> selectedDicesList = SelectedDice.Instance.GetSelectedDicesList();
        if (selectedDicesList.Count == 1 && !hasDice)
        {
            Dice selectedDice = selectedDicesList[0];
            selectedDice.HandleSelect();
            selectedDice.transform.SetParent(transform);
            selectedDice.transform.position = transform.position;
            hasDice = true;
            utilityDicesList.Add(selectedDice);
            EndModifyButton.Instance.gameObject.SetActive(true);
            EndModifyButton.Instance.unmodifiedDicesNum++;
            modifyDice = selectedDice;
            currentPip = modifyDice.GetPip();
            ShowButtons();
        }
    }

    private void HandleModify(int offset)
    {
        OnAnyDiceModified?.Invoke(this, EventArgs.Empty);

        Dice replaceDice;
        if (modifyDice.IsBlack())
        {
            replaceDice = DiceManager.Instance.GetCertainPipBlackDice(currentPip + offset);
        }
        else
        {
            replaceDice = DiceManager.Instance.GetCertainPipYellowDice(currentPip + offset);
        }
        utilityDicesList.Remove(modifyDice);
        Destroy(modifyDice.gameObject);
        modifiedDice = Instantiate(replaceDice, transform);
        modifiedDice.transform.position = transform.position;
        utilityDicesList.Add(modifiedDice);
        EndModifyButton.Instance.unmodifiedDicesNum--;
        HideButtons();
    }

    private void ShowButtons()
    {
        modifyPlusButton.gameObject.SetActive(true);
        modifyMinusButton.gameObject.SetActive(true);
    }

    private void HideButtons()
    {
        modifyPlusButton.gameObject.SetActive(false);
        modifyMinusButton.gameObject.SetActive(false);
    }
}
