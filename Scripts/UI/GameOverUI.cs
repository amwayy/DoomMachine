using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button fairyButton;
    [SerializeField] private GameObject fairy;
    [SerializeField] private Transform basicGameOverUI;

    public event EventHandler OnRevive;

    public static GameOverUI Instance { get; private set; }

    private bool hasRevived = false;
    private int basicGameOverUIOffset = 300;

    private const string GAME_SCENE = "SampleScene";

    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);

        restartButton.onClick.AddListener(() => { SceneManager.LoadScene(GAME_SCENE); });
        quitButton.onClick.AddListener(() => { Application.Quit(); });
        fairyButton.onClick.AddListener(HandleFairy);
    }

    private void HandleFairy()
    {
        OnRevive?.Invoke(this, EventArgs.Empty);
        hasRevived = true;
        gameObject.SetActive(false);
        basicGameOverUI.localPosition = Vector3.zero;
    }

    public void HandleGameOver(bool isWin)
    {
        if (isWin)
        {
            Debug.Log("You Win!");
            youWinText.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(false);
            fairy.SetActive(false);
        }
        // Game Over
        else
        {
            youWinText.gameObject.SetActive(false);
            if(StartUI.Instance.IsEasy() && !hasRevived)
            {
                fairy.SetActive(true);
                basicGameOverUI.localPosition = Vector3.left * basicGameOverUIOffset;
            }
            else
            {
                fairy.SetActive(false);
            }
        }
    }
}
