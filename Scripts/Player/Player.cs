using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button attackButton;

    public event EventHandler OnPlayerEndTurnButton;
    public event EventHandler OnPlayerAttack;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        endTurnButton.onClick.AddListener(() => 
        {
            OnPlayerEndTurnButton?.Invoke(this, EventArgs.Empty); 
        });

        attackButton.onClick.AddListener(() =>
        {
            OnPlayerAttack?.Invoke(this, EventArgs.Empty);
        });
    }

    private void Update()
    {
        UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
        if (GameManager.Instance.IsPlayerPhase())
        {
            endTurnButton.gameObject.SetActive(true);
            attackButton.gameObject.SetActive(true);
        }
        if (GameManager.Instance.IsEnemyPhase())
        {
            endTurnButton.gameObject.SetActive(false);
            attackButton.gameObject.SetActive(false);
        }
    }
}
