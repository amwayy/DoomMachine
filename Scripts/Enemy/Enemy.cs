using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Image selectedVisual;
    [SerializeField] private int hpMax;
    [SerializeField] private Transform hpSlot;
    [SerializeField] private GameObject visual;

    public static event EventHandler<OnMakeDamageEventArgs> OnAnyMakeDamage;
    public class OnMakeDamageEventArgs : EventArgs
    {
        public int damage;
    }
    public static event EventHandler OnAnyTakeDamage;
    public static event EventHandler<OnDiceLootEventArgs> OnAnyDiceLoot;
    public class OnDiceLootEventArgs : EventArgs
    {
        public Dice lootDice;
        public int enemyIndex;
    }
    public static event EventHandler OnAnyEnemyMoveEnd;
    public static event EventHandler OnDoomCoreDied;
    public static event EventHandler OnEnemiesMoveLeft; 
    public static event EventHandler OnEnemiesMoveRight;
    public static event EventHandler OnAnyEnemyMove;
    public static event EventHandler OnAnyEnemyRepair;

    public static void ResetStaticData()
    {
        OnAnyMakeDamage = null;
        OnAnyTakeDamage = null;
        OnAnyDiceLoot = null;
        OnAnyEnemyMoveEnd = null;
        OnDoomCoreDied = null;
        OnEnemiesMoveLeft = null;
        OnEnemiesMoveRight = null;
        OnAnyEnemyMove = null;
        OnAnyEnemyRepair = null;
    }

    public static Enemy Instance { get; private set; }

    [Serializable]
    public enum EnemyType
    {
        CerabralAmplifier,
        DesignFlaw,
        DoomCore,
        ExtinctionCollider,
        HardCycleStorage,
        ImpactNexus,
        InfernalPowerComplex,
        LogicMainframe,
        MiseryExtractor,
        PopulaceGrinder,
        PulsePools,
        RecyclingVats,
        SoulHavester,
        StasisGenerator,
        SynapticDisruptor,
        ThoughtSiphon
    }

    private int enemyIndex = 1;
    private int playerDamage = 1;
    private bool canMove = false;
    private bool isFinishMove = false;
    private bool isDamagedByInfernalPowerComplex = false;
    private bool isRightToMiseryExtractor = false;
    private bool isRepairedByRecyclingVats = false;
    private bool isBoostedByHardCycleStorage = false;
    private bool isNewSpawn = true;
    private float moveDelayBaseTimerMax = 1f;
    private float moveDelayTimerMax;
    private float moveDelayTimer;
    private float afterMoveDelayTimerMax = .5f;
    private float afterMoveDelayTimer;
    private Dice hpDice;

    private const int INFERNAL_POWER_COMPLEX_DAMAGE = 1;
    private const int MISERY_EXTRACTOR_DAMAGE = 2;
    private const int RECYCLING_VATS_REPAIR_NUM = 2;

    private void Awake()
    {
        Instance = this;

        if (GameManager.Instance.IsFirstRound())
        {
            isNewSpawn = false;
        }
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerPhaseEnd += GameManager_OnPlayerPhaseEnd;
        Player.Instance.OnPlayerAttack += Player_OnPlayerAttack;

        moveDelayTimerMax = moveDelayBaseTimerMax * enemyIndex;
        moveDelayTimer = moveDelayTimerMax;
        afterMoveDelayTimer = afterMoveDelayTimerMax;
    }

    private void Player_OnPlayerAttack(object sender, EventArgs e)
    {
        if(enemyIndex == EnemySpawner.Instance.GetSelectedEnemyIndex())
        {
            TryTakeDamage();
        }
    }

    private void GameManager_OnPlayerPhaseEnd(object sender, EventArgs e)
    {
        canMove = true;
        isDamagedByInfernalPowerComplex = false;
        isRepairedByRecyclingVats = false;
        isBoostedByHardCycleStorage = false;
        isNewSpawn = false;
        if (RecyclingVats.Instance != null)
        {
            RecyclingVats.Instance.SetIsToRepairAdjacent(false);
        }
        if (HardCycleStorage.Instance != null)
        {
            HardCycleStorage.Instance.SetIsToBoostAdjacent(false);
        }

        selectedVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateVisual();

        HandleAdjacentToRecyclingVats();

        HandleAdjacentToHardCycleStorage();

        if(InfernalPowerComplex.Instance != null && InfernalPowerComplex.Instance.partsToDamageNum > 0 && isDamagedByInfernalPowerComplex == false)
        {
            isDamagedByInfernalPowerComplex = true;
            InfernalPowerComplex.Instance.partsToDamageNum--;
            TakeDamage(INFERNAL_POWER_COMPLEX_DAMAGE);
        }

        int previousEnemyIndex = enemyIndex;
        List<Enemy> aliveEnemiesList = EnemyManager.Instance.GetAliveEnemiesList();
        for(int i = 0; i < aliveEnemiesList.Count; i++)
        {
            if(aliveEnemiesList[i].GetEnemyType() == enemyType)
            {
                enemyIndex = i + 1;
                if(enemyIndex != previousEnemyIndex)
                {
                    moveDelayTimerMax = moveDelayBaseTimerMax * enemyIndex;
                    moveDelayTimer = moveDelayTimerMax;
                }
            }
        }

        if(enemyIndex > 1 && aliveEnemiesList[enemyIndex - 2].TryGetComponent(out MiseryExtractor miseryExtractor))
        {
            isRightToMiseryExtractor = true;
            MiseryExtractor.Instance.damage = GetHPNum();
        }

        if(isRightToMiseryExtractor && MiseryExtractor.Instance.isDead)
        {
            TakeDamage(MISERY_EXTRACTOR_DAMAGE);
            MiseryExtractor.Instance.isDead = false;
        }

        if (canMove)
        {
            moveDelayTimer -= Time.deltaTime;
            if(moveDelayTimer < .8f)
            {
                selectedVisual.gameObject.SetActive(true);
                int enemyContainerFirstIndex = EnemySpawner.Instance.GetContainerFirstIndex();
                if (enemyIndex < enemyContainerFirstIndex)
                {
                    OnEnemiesMoveLeft?.Invoke(this, EventArgs.Empty);
                }
                if (enemyIndex > enemyContainerFirstIndex + 2)
                {
                    OnEnemiesMoveRight?.Invoke(this, EventArgs.Empty);
                }
            }
            if (moveDelayTimer < 0f)
            {
                HandleMove();
                isFinishMove = true;
                canMove = false;
                moveDelayTimer = moveDelayTimerMax;
            }
        }

        if (isFinishMove)
        {
            afterMoveDelayTimer -= Time.deltaTime;
            if (afterMoveDelayTimer < 0f)
            {
                OnAnyEnemyMoveEnd?.Invoke(this, EventArgs.Empty);
                afterMoveDelayTimer = afterMoveDelayTimerMax;
                selectedVisual.gameObject.SetActive(false);
                isFinishMove = false;
            }
        }
    }

    private void HandleAdjacentToHardCycleStorage()
    {
        if (HardCycleStorage.Instance != null)
        {
            HardCycleStorage hardCycleStorage = HardCycleStorage.Instance;
            bool isAdjacentToHardCycleStorage = enemyIndex == hardCycleStorage.GetEnemyIndex() - 1 || enemyIndex == hardCycleStorage.GetEnemyIndex() + 1;
            if (hardCycleStorage.IsToBoostAdjacent() && isAdjacentToHardCycleStorage && !isBoostedByHardCycleStorage && !isNewSpawn)
            {
                HandleMove();
                isBoostedByHardCycleStorage = true;
            }
        }
    }

    public int GetEnemyIndex()
    {
        return enemyIndex;
    }

    private void HandleAdjacentToRecyclingVats()
    {
        if (RecyclingVats.Instance != null)
        {
            RecyclingVats recyclingVats = RecyclingVats.Instance;
            bool isAdjacentToRecyclingVats = enemyIndex == recyclingVats.GetEnemyIndex() - 1 || enemyIndex == recyclingVats.GetEnemyIndex() + 1;
            if(recyclingVats.IsToRepairAdjacent() && isAdjacentToRecyclingVats && !isRepairedByRecyclingVats)
            {
                Repair(RECYCLING_VATS_REPAIR_NUM);
                isRepairedByRecyclingVats = true;
            }
        }
    }

    public void LimitShield()
    {
        Shield.Instance.SetIsShieldLimited(true);
    }

    private void UpdateVisual()
    {
        int containerFirstIndex = EnemySpawner.Instance.GetContainerFirstIndex();
        // List<Enemy> aliveEnemiesList = EnemyManager.Instance.GetAliveEnemiesList();
        if (enemyIndex < containerFirstIndex || enemyIndex - containerFirstIndex > 2)
        {
            visual.SetActive(false);
        }
        else
        {
            visual.SetActive(true);
        }

        if (GameManager.Instance.IsPlayerPhase())
        {
            if(enemyIndex == EnemySpawner.Instance.GetSelectedEnemyIndex())
            {
                selectedVisual.gameObject.SetActive(true);
            }
            else
            {
                selectedVisual.gameObject.SetActive(false);
            }
            // Debug.Log("This Enemy Index: " + enemyIndex + "; Selected Enemy Index: " + EnemySpawner.Instance.GetSelectedEnemyIndex());
        }
    }

    public int GetHPNum()
    {
        return hpDice.GetPip();
    }

    public void ModifySentiencePower(int modifyNum)
    {
        Sentience.Instance.ModifySentience(modifyNum);
        Power.Instance.ModifyPower(modifyNum);
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public void Repair(int repairNum)
    {
        OnAnyEnemyRepair?.Invoke(this, EventArgs.Empty);

        int currentHP = hpDice.GetPip();
        Dice repairedHPDice;
        if (currentHP + repairNum >= hpMax)
        {
            repairedHPDice = DiceManager.Instance.GetCertainPipYellowDice(hpMax);
        }
        else
        {
            repairedHPDice = DiceManager.Instance.GetCertainPipYellowDice(currentHP + repairNum);
        }
        Transform parent = hpDice.transform.parent;
        Destroy(hpDice.gameObject);
        repairedHPDice = Instantiate(repairedHPDice, parent);
        repairedHPDice.transform.position = parent.position;
        hpDice = repairedHPDice;
    }

    public void MakeDamage(int damage)
    {
        OnAnyMakeDamage?.Invoke(this, new OnMakeDamageEventArgs { damage = damage });
    }

    public virtual void HandleMove()
    {
        OnAnyEnemyMove?.Invoke(this, EventArgs.Empty);
    }

    public virtual void EnemyDied()
    {
        Dice lootDice = DiceManager.Instance.GetCertainPipYellowDice(hpDice.GetPip());
        OnAnyDiceLoot?.Invoke(this, new OnDiceLootEventArgs { lootDice = lootDice, enemyIndex = enemyIndex });

        if(enemyType == EnemyType.DoomCore)
        {
            OnDoomCoreDied?.Invoke(this, EventArgs.Empty);
        }

        GameManager.Instance.OnPlayerPhaseEnd -= GameManager_OnPlayerPhaseEnd;
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        OnAnyTakeDamage?.Invoke(this, EventArgs.Empty);
        int currenthp = hpDice.GetPip();
        if (currenthp <= damage)
        {
            EnemyDied();
        }
        else
        {
            Destroy(hpDice.gameObject);
            Dice replaceDice = DiceManager.Instance.GetCertainPipYellowDice(currenthp - damage);
            replaceDice = Instantiate(replaceDice, hpSlot);
            replaceDice.transform.position = hpSlot.position;
            hpDice = replaceDice;
        }
    }

    public void TryTakeDamage()
    {
        List<Dice> selectedDicesList = SelectedDice.Instance.GetSelectedDicesList();
        if (CanTakeDamage(selectedDicesList))
        {
            TakeDamage(playerDamage);
        }
    }

    public virtual bool CanTakeDamage(List<Dice> selectedDicesList)
    {
        return true;
    }

    public void InitiateHP()
    {
        hpDice = Instantiate(DiceManager.Instance.GetCertainPipYellowDice(hpMax), hpSlot.transform);
        hpDice.transform.position = hpSlot.transform.position;
    }
}
