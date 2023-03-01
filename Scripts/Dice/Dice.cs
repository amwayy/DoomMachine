using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private Image selectedVisual;
    [SerializeField] private Image lockedVisual;
    [SerializeField] private Button diceButton;
    [SerializeField] private int pip;
    [SerializeField] private DiceColor diceColor;

    public static event EventHandler<OnDiceSelectedEventArgs> OnAnyDiceSelected;
    public class OnDiceSelectedEventArgs : EventArgs
    {
        public Dice dice;
    }
    public static event EventHandler<OnDiceUnselectedEventArgs> OnAnyDiceUnselected;
    public class OnDiceUnselectedEventArgs : EventArgs
    {
        public Dice dice;
    }

    public static void ResetStaticData()
    {
        OnAnyDiceUnselected = null;
        OnAnyDiceSelected = null;
    }

    public static Dice Instance { get; private set; }

    private bool isSelected = false;
    private bool isLocked = false;
    private int lockCountdown = 0;

    [Serializable]
    public enum DiceColor
    {
        Black,
        Yellow
    }

    private void Awake()
    {
        Instance = this;

        selectedVisual.gameObject.SetActive(false);
        lockedVisual.gameObject.SetActive(false);

        diceButton.onClick.AddListener(() => { HandleSelect(); });
    }

    public void SetLocked()
    {
        isLocked = true;
        lockedVisual.gameObject.SetActive(true);
        lockCountdown = 1;
    }

    public void SetUnlocked()
    {
        isLocked = false;
        lockedVisual.gameObject.SetActive(false);
    }

    public void LockCountDown()
    {
        lockCountdown--;
    }

    public int GetLockCountdown()
    {
        return lockCountdown;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public bool IsBlack()
    {
        return diceColor == DiceColor.Black;
    }

    public bool IsYellow()
    {
        return diceColor == DiceColor.Yellow;
    }

    public void SetSelectVisual(bool isSelected)
    {
        if (isSelected)
        {
            selectedVisual.gameObject.SetActive(true);
        }
        else
        {
            selectedVisual.gameObject.SetActive(false);
        }
    }

    public void HandleSelect()
    {
        if(transform.parent.TryGetComponent(out DiceSpawner parent))
        {
            isSelected = !isSelected;
            SetSelectVisual(isSelected);
            if (isSelected)
            {
                OnAnyDiceSelected?.Invoke(this, new OnDiceSelectedEventArgs { dice = this });
            }
            else
            {
                OnAnyDiceUnselected?.Invoke(this, new OnDiceUnselectedEventArgs { dice = this });
            }
        }
    }

    public int GetPip()
    {
        return pip;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
