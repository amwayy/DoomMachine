using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        Enemy.ResetStaticData();
        Dice.ResetStaticData();
        Reroll.ResetStaticData();
        Modify.ResetStaticData();
    }
}
