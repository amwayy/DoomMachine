using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sentience : MonoBehaviour
{
    [SerializeField] private List<Image> sentienceSlotsList;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TextMeshProUGUI sentienceNumText;

    public event EventHandler OnSentienceModify;

    public static Sentience Instance { get; private set; }

    private int sentience = 0;
    private int sentienceMax = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < sentienceMax; i++)
        {
            if (i < sentience)
            {
                sentienceSlotsList[i].color = activeColor;
            }
            else
            {
                sentienceSlotsList[i].color = inactiveColor;
            }
        }
        sentienceNumText.text = sentience.ToString();
    }

    public int GetSentience()
    {
        return sentience;
    }

    public void ModifySentience(int modifyNum)
    {
        OnSentienceModify?.Invoke(this, EventArgs.Empty);

        sentience += modifyNum;
        if(sentience < 0)
        {
            sentience = 0;
        }
        if(sentience > sentienceMax)
        {
            sentience = sentienceMax;
        }
        UpdateVisual();
    }
}
