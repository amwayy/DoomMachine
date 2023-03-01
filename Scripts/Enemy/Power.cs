using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Power : MonoBehaviour
{
    [SerializeField] private List<Image> powerSlotsList;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TextMeshProUGUI powerNumText;

    public event EventHandler OnPowerModify;

    public static Power Instance { get; private set; }

    private int power = 0;
    private int powerMax = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < powerMax; i++)
        {
            if(i < power)
            {
                powerSlotsList[i].color = activeColor;
            }
            else 
            {
                powerSlotsList[i].color = inactiveColor;
            }
        }
        powerNumText.text = power.ToString();
    }

    public int GetPower()
    {
        return power;
    }

    public void ModifyPower(int modifyNum)
    {
        OnPowerModify?.Invoke(this, EventArgs.Empty);

        power += modifyNum;
        if(power < 0)
        {
            power = 0;
        }
        if(power > powerMax)
        {
            power = powerMax;
        }
        UpdateVisual();
    }
}
