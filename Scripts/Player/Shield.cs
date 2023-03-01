using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shield : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shieldNumText;
    [SerializeField] private Transform usedDicesArea;
    [SerializeField] private GameObject shieldLockedVisual;
    [SerializeField] private Button shieldSlotsButton;
    [SerializeField] private List<Transform> shieldSlotsList;

    public event EventHandler<OnShieldAllLostEventArgs> OnShieldAllLost;
    public class OnShieldAllLostEventArgs : EventArgs
    {
        public int realDamage;
    }
    public event EventHandler OnShieldGuard;
    public event EventHandler OnDicesUsedUpByShield;
    public event EventHandler OnShieldUp;

    public static Shield Instance { get; private set; }

    private int shieldNum = 0;
    private int realDamage = 0;
    private int shieldSlotsNum = 4;
    private int availableShieldSlotsNum = 4;
    private List<Dice> shieldDicesList;
    private bool isShieldLimited = false;
    private bool hasPutShield = false;

    private void Awake()
    {
        Instance = this;

        shieldDicesList = new List<Dice>();

        shieldSlotsButton.onClick.AddListener(HandleAddShield);
    }

    private void Start()
    {
        Enemy.OnAnyMakeDamage += Enemy_OnMakeDamage;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        GameManager.Instance.OnPlayerPhaseEnd += GameManager_OnPlayerPhaseEnd;
        StartUI.Instance.OnStart += StartUI_OnStart;
    }

    private void StartUI_OnStart(object sender, EventArgs e)
    {
        if (StartUI.Instance.IsHard())
        {
            shieldSlotsNum = 3;
            shieldSlotsList[3].gameObject.SetActive(false);
            shieldSlotsList.RemoveAt(3);
        }
    }

    private void GameManager_OnPlayerPhaseEnd(object sender, EventArgs e)
    {
        isShieldLimited = false;
        hasPutShield = false;
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, EventArgs e)
    {
        // Reroll
        int shieldDicesNum = shieldDicesList.Count;
        for(int i = 0; i < shieldDicesNum; i++)
        {
            Dice shieldDice = shieldDicesList[0];
            shieldNum -= shieldDice.GetPip();
            Transform parentTransform = shieldDice.transform.parent;
            shieldDicesList.Remove(shieldDice);
            Destroy(shieldDice.gameObject);
            int randomPip = UnityEngine.Random.Range(1, 7);
            Dice replaceDice = DiceManager.Instance.GetCertainPipBlackDice(randomPip);
            Dice rerolledDice = Instantiate(replaceDice, parentTransform);
            rerolledDice.transform.position = parentTransform.position;
            shieldDicesList.Add(rerolledDice);
            shieldNum += replaceDice.GetPip();
            shieldNumText.text = shieldNum.ToString();
        }
    }

    private void Enemy_OnMakeDamage(object sender, Enemy.OnMakeDamageEventArgs e)
    {
        TakeDamage(e.damage);
    }

    private void Update()
    {
        if (isShieldLimited)
        {
            shieldLockedVisual.SetActive(true);
        }
        else
        {
            shieldLockedVisual.SetActive(false);
        }
    }

    private void HandleAddShield()
    {
        if(isShieldLimited && hasPutShield)
        {
            Debug.Log("Shield Dice Limited and You Have Put One Shield Dice.");
            return;
        }

        availableShieldSlotsNum = shieldSlotsNum - shieldDicesList.Count;
        if (isShieldLimited)
        {
            availableShieldSlotsNum = Math.Min(availableShieldSlotsNum, 1);
            Debug.Log("Shield Dice Limited. Current Available Shield Slots Num: " + availableShieldSlotsNum);
        }
        List<Dice> selectedDicesList = SelectedDice.Instance.GetSelectedDicesList();
        int selectedDicesNum = selectedDicesList.Count;
        if(selectedDicesNum > 0 && selectedDicesNum <= availableShieldSlotsNum)
        {
            Debug.Log("You Can Put These Selected Dices as Shield Dices.");

            OnShieldUp?.Invoke(this, EventArgs.Empty);

            for(int i = 0; i < selectedDicesNum; i++)
            {
                Dice selectedDice = selectedDicesList[0];
                selectedDice.HandleSelect();
                for(int j = 0; j < shieldSlotsNum; j++)
                {
                    if(shieldSlotsList[j].childCount == 0)
                    {
                        Transform firstAvailableShieldSlot = shieldSlotsList[j];
                        selectedDice.transform.SetParent(firstAvailableShieldSlot);
                        selectedDice.transform.position = firstAvailableShieldSlot.position;
                        break;
                    }
                }
                shieldNum += selectedDice.GetPip();
                shieldNumText.text = shieldNum.ToString();
                shieldDicesList.Add(selectedDice);
            }
            if (DiceSpawner.Instance.GetAvailableDicesNum() == 0)
            {
                OnDicesUsedUpByShield?.Invoke(this, EventArgs.Empty);
            }
            hasPutShield = true;
        }
    }

    public void SetIsShieldLimited(bool isLimited)
    {
        isShieldLimited = isLimited;
    }

    public List<Dice> GetShieldDicesList()
    {
        return shieldDicesList;
    }

    private void TakeDamage(int damage)
    {
        if(shieldNum - damage >= 0 && shieldNum != 0)
        {
            OnShieldGuard?.Invoke(this, EventArgs.Empty);
        }

        // ¶Ü×ã¹»µÖµ²ÉËº¦
        // Debug.Log("ShieldNum: " + shieldNum + "; ShieldDicesNum" + shieldDicesList.Count + "; Damage: " + damage);
        if (shieldNum - damage > 0)
        {
            // Debug.Log("Enough Shield");
            shieldNum -= damage;
            shieldNumText.text = shieldNum.ToString();
            int shieldDicesNum = shieldDicesList.Count;
            int remainingFirstDiceIndex = 0;
            int remainingFirstShieldNum = 0;
            for(int i = 0; i < shieldDicesNum; i++)
            {
                int firstIShieldNum = 0;
                for(int j = 0; j < i; j++)
                {
                    firstIShieldNum += shieldDicesList[j].GetPip();
                }
                int firstIPlus1ShieldNum = firstIShieldNum + shieldDicesList[i].GetPip();
                if(firstIShieldNum <= damage && firstIPlus1ShieldNum > damage)
                {
                    remainingFirstDiceIndex = i;
                    remainingFirstShieldNum = firstIPlus1ShieldNum - damage;
                }
            }
            Transform remainingFirstDiceParent = shieldDicesList[remainingFirstDiceIndex].transform.parent;
            Dice lostShieldDice;
            for (int i = 0; i < remainingFirstDiceIndex; i++)
            {
                lostShieldDice = shieldDicesList[0];
                shieldDicesList.RemoveAt(0);
                lostShieldDice.transform.SetParent(usedDicesArea);
                UsedDices.Instance.UpdateUsedDicesList();
            }
            lostShieldDice = shieldDicesList[0];
            Dice replaceDice = null;
            if (lostShieldDice.IsBlack())
            {
                replaceDice = DiceManager.Instance.GetCertainPipBlackDice(remainingFirstShieldNum);
            }
            if (lostShieldDice.IsYellow())
            {
                replaceDice = DiceManager.Instance.GetCertainPipYellowDice(remainingFirstShieldNum);
            }
            shieldDicesList.RemoveAt(0);
            Destroy(lostShieldDice.gameObject);
            replaceDice = Instantiate(replaceDice, remainingFirstDiceParent);
            replaceDice.transform.position = remainingFirstDiceParent.transform.position;
            shieldDicesList.Insert(0, replaceDice);
        }
        // ¶ÜÈ«ÆÆ£¬Òç³öÉËº¦×ªÎªÕæÉË
        else
        {
            // Debug.Log("Shield All Lost");
            foreach(Dice shieldDice in shieldDicesList)
            {
                shieldDice.transform.SetParent(usedDicesArea);
                UsedDices.Instance.UpdateUsedDicesList();
            }
            shieldDicesList.Clear();
            realDamage = damage - shieldNum;
            OnShieldAllLost?.Invoke(this, new OnShieldAllLostEventArgs { realDamage = realDamage });
            shieldNum = 0;
            shieldNumText.text = shieldNum.ToString();
        }
        // Debug.Log("ShieldNum: " + shieldNum + "; ShieldDicesNum" + shieldDicesList.Count);
    }
}
