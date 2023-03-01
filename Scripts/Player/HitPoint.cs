using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitPoint : MonoBehaviour
{
    [SerializeField] private List<Image> hpSlotList;
    [SerializeField] private TextMeshProUGUI hpNumText;
    [SerializeField] private Color lostHPColor;
    [SerializeField] private Color keptHPColor;

    public event EventHandler OnPlayerDied;
    public event EventHandler OnPlayerDamaged;

    public static HitPoint Instance { get; private set; }

    private int hpNum = 10;

    private const int HP_NUM_MAX = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Shield.Instance.OnShieldAllLost += Shield_OnShieldAllLost;
        GameOverUI.Instance.OnRevive += GameOverUI_OnRevive;
    }

    private void GameOverUI_OnRevive(object sender, EventArgs e)
    {
        hpNum = 3;
        HandleVisual();
    }

    private void Shield_OnShieldAllLost(object sender, Shield.OnShieldAllLostEventArgs e)
    {
        TakeRealDamage(e.realDamage);
    }

    private void TakeRealDamage(int damage)
    {
        // Debug.Log("HP: " + hpNum + "; Damage: " + damage);
        if(hpNum - damage > 0)
        {
            if(damage > 0)
            {
                OnPlayerDamaged?.Invoke(this, EventArgs.Empty);
            }
            hpNum -= damage;
            HandleVisual();
        }
        else
        {
            hpNum = 0;
            HandleVisual();
            // Game Over!
            OnPlayerDied?.Invoke(this, EventArgs.Empty);
        }
        // Debug.Log("HP: " + hpNum);
    }

    private void HandleVisual()
    {
        for(int i = 0; i < HP_NUM_MAX; i++)
        {
            if(i < hpNum)
            {
                hpSlotList[i].color = keptHPColor;
            }
            else
            {
                hpSlotList[i].color = lostHPColor;
            }
        }
        hpNumText.text = hpNum.ToString();
    }
}
