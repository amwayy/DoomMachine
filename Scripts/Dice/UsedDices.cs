using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedDices : MonoBehaviour
{
    public event EventHandler<OnDiceLockedEventArgs> OnDiceLocked;
    public class OnDiceLockedEventArgs : EventArgs
    {
        public Dice lockedDice;
    }
    public event EventHandler<OnDiceUnlockedEventArgs> OnDiceUnlocked;
    public class OnDiceUnlockedEventArgs : EventArgs
    {
        public Dice unlockedDice;
    }

    public static UsedDices Instance { get; private set; }

    private List<Dice> usedDicesList;

    private void Awake()
    {
        Instance = this;

        usedDicesList = new List<Dice>();
    }

    private void Start()
    {
        Enemy.OnAnyDiceLoot += Enemy_OnDiceLoot;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
    }

    //private void Update()
    //{
    //    UpdateUsedDicesList();
    //}

    public void LockDice()
    {
        UpdateUsedDicesList();
        foreach (Dice usedDice in usedDicesList)
        {
            if (!usedDice.IsLocked())
            {
                usedDice.SetLocked();
                OnDiceLocked?.Invoke(this, new OnDiceLockedEventArgs { lockedDice = usedDice });
                break;
            }
        }
    }

    public List<Dice> GetUsedDicesList()
    {
        return usedDicesList;
    }

    public void UpdateUsedDicesList()
    {
        usedDicesList.Clear();
        foreach (Transform child in transform)
        {
            child.TryGetComponent(out Dice usedDice);
            usedDicesList.Add(usedDice);
        }
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, System.EventArgs e)
    {
        foreach(Dice usedDice in usedDicesList)
        {
            if (usedDice.IsLocked())
            {
                if(usedDice.GetLockCountdown() == 0)
                {
                    usedDice.SetUnlocked();
                    OnDiceUnlocked?.Invoke(this, new OnDiceUnlockedEventArgs { unlockedDice = usedDice });
                }
                else
                {
                    usedDice.LockCountDown();
                }
            }
        }
        ClearDices();
    }

    private void Enemy_OnDiceLoot(object sender, Enemy.OnDiceLootEventArgs e)
    {
        Instantiate(e.lootDice, transform);
    }

    private void ClearDices()
    {
        foreach (Dice usedDice in usedDicesList)
        {
            if (!usedDice.IsLocked())
            {
                Destroy(usedDice.gameObject);
            }
        }
        UpdateUsedDicesList();
    }
}
