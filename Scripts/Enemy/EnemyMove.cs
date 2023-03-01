using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject moveEffectDetail;

    private bool isShowDetail = true;
    private Image enemyMove;

    private void Awake()
    {
        enemyMove = GetComponent<Image>();
    }

    private void Start()
    {
        isShowDetail = PauseUI.Instance.IsShowEnemyMoveDetail();

        if (!isShowDetail)
        {
            enemyMove.raycastTarget = false;
        }

        PauseUI.Instance.OnChangeToggle += PauseUI_OnChangeToggle;
    }

    private void PauseUI_OnChangeToggle(object sender, System.EventArgs e)
    {
        isShowDetail = PauseUI.Instance.IsShowEnemyMoveDetail();
        if (isShowDetail)
        {
            enemyMove.raycastTarget = true;
        }
        else
        {
            enemyMove.raycastTarget = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isShowDetail)
        {
            moveEffectDetail.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        moveEffectDetail.SetActive(false);
    }
}
