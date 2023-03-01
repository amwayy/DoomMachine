using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private OptionsUI optionsUI;

    AudioSource music;

    private const string MUSIC_VOLUME = "MusicVolume";

    private void Awake()
    {
        music = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            music.volume = PlayerPrefs.GetInt(MUSIC_VOLUME);
        }
    }

    private void Start()
    {
        optionsUI.OnMusicVolumeChanged += OptionsUI_OnVolumeChanged;
    }

    private void OptionsUI_OnVolumeChanged(object sender, System.EventArgs e)
    {
        music.volume = OptionsUI.Instance.GetMusicVolumeNormalized();
    }
}
