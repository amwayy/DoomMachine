using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DifficultyText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform difficultyDetail;
    [SerializeField] private TextMeshProUGUI difficultyDetailText;

    private const string EASY_TEXT = "�㽫���\n�ڶ��λ���";
    private const string MEDIUM_TEXT = "��׼�Ѷ�\n(4�����ܲ�\n6��������)";
    private const string HARD_TEXT = "���ܲۼ�Ϊ3��\n�����ۼ�Ϊ4��";

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
