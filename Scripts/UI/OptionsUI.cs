using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button musicVolumeDownButton;
    [SerializeField] private Button musicVolumeUpButton;
    [SerializeField] private Button soundEffectVolumeDownButton;
    [SerializeField] private Button soundEffectVolumeUpButton;
    [SerializeField] private TextMeshProUGUI musicVolumeValueText;
    [SerializeField] private TextMeshProUGUI soundEffectSoundValueText;
    [SerializeField] private Toggle fullScreenToggle;

    public event EventHandler OnMusicVolumeChanged;
    public event EventHandler OnSoundEffectVolumeChanged;

    public static OptionsUI Instance { get; private set; }

    private int musicVolume = 5;
    private int soundEffectVolume = 5;

    private const int VOLUME_MIN = 0;
    private const int VOLUME_MAX = 10;
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SOUND_EFFECT_VOLUME = "SoundEffectVolume";
    private const string IS_FULL_SCREEEN = "IsFullScreen";

    private void Awake()
    {
        Instance = this;

        musicVolumeDownButton.onClick.AddListener(() => HandleMusicVolumeDown());
        musicVolumeUpButton.onClick.AddListener(() => HandleMusicVolumeUp());
        soundEffectVolumeDownButton.onClick.AddListener(() => HandleSoundEffectVolumeDown()) ;
        soundEffectVolumeUpButton.onClick.AddListener(() => HandleSoundEffectVolumeUp());

        fullScreenToggle.onValueChanged.AddListener(call => 
        {
            Screen.fullScreen = call;
            if (call)
            {
                PlayerPrefs.SetInt(IS_FULL_SCREEEN, 1);
                Debug.Log("FullScreen");
            }
            else
            {
                PlayerPrefs.SetInt(IS_FULL_SCREEEN, 0);
                Debug.Log("Windowed");
            }
        });

        if (PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            musicVolume = PlayerPrefs.GetInt(MUSIC_VOLUME);
            musicVolumeValueText.text = musicVolume.ToString();
        }
        if (PlayerPrefs.HasKey(SOUND_EFFECT_VOLUME))
        {
            soundEffectVolume = PlayerPrefs.GetInt(SOUND_EFFECT_VOLUME);
            soundEffectSoundValueText.text = soundEffectVolume.ToString();
        }

        fullScreenToggle.isOn = Screen.fullScreen;
    }

    private void HandleSoundEffectVolumeDown()
    {
        if (soundEffectVolume > VOLUME_MIN)
        {
            soundEffectVolume--;
            PlayerPrefs.SetInt(SOUND_EFFECT_VOLUME, soundEffectVolume);
            soundEffectSoundValueText.text = soundEffectVolume.ToString();
        }
        OnSoundEffectVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleSoundEffectVolumeUp()
    {
        if (soundEffectVolume < VOLUME_MAX)
        {
            soundEffectVolume++;
            PlayerPrefs.SetInt(SOUND_EFFECT_VOLUME, soundEffectVolume);
            soundEffectSoundValueText.text = soundEffectVolume.ToString();
        }
        OnSoundEffectVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleMusicVolumeDown()
    {
        if (musicVolume > VOLUME_MIN)
        {
            musicVolume--;
            PlayerPrefs.SetInt(MUSIC_VOLUME, musicVolume);
            musicVolumeValueText.text = musicVolume.ToString();
        }
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleMusicVolumeUp()
    {
        if (musicVolume < VOLUME_MAX)
        {
            musicVolume++;
            PlayerPrefs.SetInt(MUSIC_VOLUME, musicVolume);
            musicVolumeValueText.text = musicVolume.ToString();
        }
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetMusicVolumeNormalized()
    {
        return (float)musicVolume / 10;
    }

    public float GetSoundEffectVolumeNormalized()
    {
        return (float)soundEffectVolume / 10;
    }
}
