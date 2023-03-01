using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private OptionsUI optionsUI;

    [SerializeField] private AudioClip selectToggleDiceSound;
    [SerializeField] private AudioClip rollDicesSound;
    [SerializeField] private AudioClip rerollDiceSound;
    [SerializeField] private AudioClip shieldGuardSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip gameWinSound;
    [SerializeField] private AudioClip enemyMoveSound;
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip enemyDieSound;
    [SerializeField] private AudioClip lockDiceSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip statUpSound;
    [SerializeField] private AudioClip reviveSound;

    private AudioSource audioSource;
    private float volume = .5f;

    private const string SOUND_EFFECT_VOLUME = "SoundEffectVolume";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey(SOUND_EFFECT_VOLUME))
        {
            volume = PlayerPrefs.GetInt(SOUND_EFFECT_VOLUME);
        }
    }

    private void Start()
    {
        optionsUI.OnSoundEffectVolumeChanged += OptionsUI_OnSoundEffectVolumeChanged;

        optionsUI.OnMusicVolumeChanged += OptionsUI_OnMusicVolumeChanged;
        Dice.OnAnyDiceSelected += Dice_OnAnyDiceSelected;
        Dice.OnAnyDiceUnselected += Dice_OnAnyDiceUnselected;
        EnemyManager.Instance.OnEnemyPhaseEnd += EnemyManager_OnEnemyPhaseEnd;
        StartUI.Instance.OnStart += StartUI_OnStart;
        Reroll.OnAnyDiceReroll += Reroll_OnAnyDiceReroll;
        Modify.OnAnyDiceModified += Modify_OnAnyDiceModified;
        Shield.Instance.OnShieldGuard += Shield_OnShieldGuard;
        Enemy.OnAnyTakeDamage += Enemy_OnAnyTakeDamage;
        HitPoint.Instance.OnPlayerDamaged += HitPoint_OnPlayerDamaged;
        HitPoint.Instance.OnPlayerDied += HitPoint_OnPlayerDied;
        Enemy.OnAnyEnemyMove += Enemy_OnAnyEnemyMove;
        Enemy.OnAnyDiceLoot += Enemy_OnAnyDiceLoot;
        UsedDices.Instance.OnDiceLocked += UsedDices_OnDiceLocked;
        UsedDices.Instance.OnDiceUnlocked += UsedDices_OnDiceUnlocked;
        Enemy.OnAnyEnemyRepair += Enemy_OnAnyEnemyRepair;
        Enemy.OnDoomCoreDied += Enemy_OnDoomCoreDied;
        Sentience.Instance.OnSentienceModify += Sentience_OnSentienceModify;
        Power.Instance.OnPowerModify += Power_OnPowerModify;
        Player.Instance.OnPlayerEndTurnButton += Player_OnPlayerEndTurnButton;
        EndModifyButton.Instance.OnEndModify += EndModifyButton_OnEndModify;
        PauseUI.Instance.OnResume += PauseUI_OnResume;
        Shield.Instance.OnShieldUp += Shield_OnShieldUp;
        EnemySpawner.Instance.OnMoveLeftButtonPressed += EnemySpawner_OnMoveLeftButtonPressed;
        EnemySpawner.Instance.OnMoveRightButtonPressed += EnemySpawner_OnMoveRightButtonPressed;
        PauseUI.Instance.OnChangeToggle += PauseUI_OnChangeToggle;
        StartUI.Instance.OnStartSetting += StartUI_OnStartSetting;
        GameOverUI.Instance.OnRevive += GameOverUI_OnRevive;
    }

    private void GameOverUI_OnRevive(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(reviveSound, volume);
    }

    private void StartUI_OnStartSetting(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void PauseUI_OnChangeToggle(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void OptionsUI_OnMusicVolumeChanged(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void OptionsUI_OnSoundEffectVolumeChanged(object sender, System.EventArgs e)
    {
        volume = OptionsUI.Instance.GetSoundEffectVolumeNormalized();
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void EnemySpawner_OnMoveRightButtonPressed(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void EnemySpawner_OnMoveLeftButtonPressed(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void Shield_OnShieldUp(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(selectToggleDiceSound, volume);
    }

    private void PauseUI_OnResume(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void EndModifyButton_OnEndModify(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void Player_OnPlayerEndTurnButton(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(buttonClickSound, volume);
    }

    private void Power_OnPowerModify(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(statUpSound, volume);
    }

    private void Sentience_OnSentienceModify(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(statUpSound, volume);
    }

    private void Enemy_OnDoomCoreDied(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(gameWinSound, volume);
    }

    private void Enemy_OnAnyEnemyRepair(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(healSound, volume);
    }

    private void UsedDices_OnDiceUnlocked(object sender, UsedDices.OnDiceUnlockedEventArgs e)
    {
        audioSource.PlayOneShot(lockDiceSound, volume);
    }

    private void UsedDices_OnDiceLocked(object sender, UsedDices.OnDiceLockedEventArgs e)
    {
        audioSource.PlayOneShot(lockDiceSound, volume);
    }

    private void Enemy_OnAnyDiceLoot(object sender, Enemy.OnDiceLootEventArgs e)
    {
        audioSource.PlayOneShot(enemyDieSound, volume);
    }

    private void Enemy_OnAnyEnemyMove(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(enemyMoveSound, volume);
    }

    private void HitPoint_OnPlayerDied(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(gameOverSound, volume);
    }

    private void HitPoint_OnPlayerDamaged(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(damageSound, volume);
    }

    private void Enemy_OnAnyTakeDamage(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(playerAttackSound, volume);
    }

    private void Shield_OnShieldGuard(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(shieldGuardSound, volume);
    }

    private void Modify_OnAnyDiceModified(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(selectToggleDiceSound, volume);
    }

    private void Reroll_OnAnyDiceReroll(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(rerollDiceSound, volume);
    }

    private void StartUI_OnStart(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(rollDicesSound, volume);
    }

    private void EnemyManager_OnEnemyPhaseEnd(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(rollDicesSound, volume);
    }

    private void Dice_OnAnyDiceUnselected(object sender, Dice.OnDiceUnselectedEventArgs e)
    {
        audioSource.PlayOneShot(selectToggleDiceSound, volume);
    }

    private void Dice_OnAnyDiceSelected(object sender, Dice.OnDiceSelectedEventArgs e)
    {
        audioSource.PlayOneShot(selectToggleDiceSound, volume);
    }
}
