using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyNumModify : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform enemyNumDetail;

    public void OnPointerEnter(PointerEventData eventData)
    {
        enemyNumDetail.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enemyNumDetail.gameObject.SetActive(false);
    }
}
