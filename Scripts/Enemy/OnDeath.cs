using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnDeath : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject onDeathDetail;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onDeathDetail.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onDeathDetail.SetActive(false);
    }
}
