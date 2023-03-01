using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Toggle detailToggle;

    private bool isShowEnemyMoveDetail = true;

    public event EventHandler OnResume;
    public event EventHandler OnChangeToggle;

    public static PauseUI Instance { get; private set; }

    private const string GAME_SCENE = "SampleScene";
    private const string IS_SHOW_DETAIL = "IsShowDetail";

    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);

        resumeButton.onClick.AddListener(() => { OnResume?.Invoke(this, EventArgs.Empty); });
        restartButton.onClick.AddListener(() => { SceneManager.LoadScene(GAME_SCENE); });
        quitButton.onClick.AddListener(() => { Application.Quit(); });

        detailToggle.onValueChanged.AddListener(call => { HandleDetailToggle(call); });

        if (PlayerPrefs.HasKey(IS_SHOW_DETAIL))
        {
            if (PlayerPrefs.GetInt(IS_SHOW_DETAIL) == 0)
            {
                isShowEnemyMoveDetail = false;
            }
            if (PlayerPrefs.GetInt(IS_SHOW_DETAIL) == 1)
            {
                isShowEnemyMoveDetail = true;
            }
        }
        detailToggle.isOn = isShowEnemyMoveDetail;
    }

    public bool IsShowEnemyMoveDetail()
    {
        return isShowEnemyMoveDetail;
    }

    private void HandleDetailToggle(bool isToggled)
    {
        if (isToggled)
        {
            isShowEnemyMoveDetail = true;
            PlayerPrefs.SetInt(IS_SHOW_DETAIL, 1);
        }
        else
        {
            isShowEnemyMoveDetail = false;
            PlayerPrefs.SetInt(IS_SHOW_DETAIL, 0);
        }

        OnChangeToggle?.Invoke(this, EventArgs.Empty);
    }
}
