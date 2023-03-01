using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DifficultyText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform difficultyDetail;
    [SerializeField] private TextMeshProUGUI difficultyDetailText;

    private const string EASY_TEXT = "你将获得\n第二次机会";
    private const string MEDIUM_TEXT = "标准难度\n(4个护盾槽\n6个修正槽)";
    private const string HARD_TEXT = "护盾槽减为3个\n修正槽减为4个";

    public void OnPointerEnter(PointerEventData eventData)
    {
        difficultyDetail.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        difficultyDetail.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (StartUI.Instance.IsEasy())
        {
            difficultyDetailText.text = EASY_TEXT;
        }
        if (StartUI.Instance.IsMedium())
        {
            difficultyDetailText.text = MEDIUM_TEXT;
        }
        if (StartUI.Instance.IsHard())
        {
            difficultyDetailText.text = HARD_TEXT;
        }
    }
}
