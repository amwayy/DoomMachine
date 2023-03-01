using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndModifyButton : MonoBehaviour
{
    public event EventHandler OnEndModify;

    public static EndModifyButton Instance { get; private set; }

    public int unmodifiedDicesNum = 0;

    private Button endModifyButton;

    private void Awake()
    {
        Instance = this;

        endModifyButton = GetComponent<Button>();
        endModifyButton.gameObject.SetActive(false);

        endModifyButton.onClick.AddListener(() => 
        {
            UsedDices.Instance.UpdateUsedDicesList();
            HandleEndModify();
            Debug.Log("Used Dices Count: " + UsedDices.Instance.GetUsedDicesList().Count) ;
        });
    }

    private void HandleEndModify()
    {
        if (unmodifiedDicesNum == 0)
        {
            OnEndModify?.Invoke(this, EventArgs.Empty);
            endModifyButton.gameObject.SetActive(false);
            Utility.Instance.GetUtilityDicesList().Clear();
        }
    }
}
