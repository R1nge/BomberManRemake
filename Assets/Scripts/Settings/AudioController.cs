using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings
{
    public class AudioController : MonoBehaviour
    {
        public event Action<int, int, int> OnSettingsLoaded;
        [SerializeField] private AudioMixer audioMixer;
        private const string MASTER_VOLUME = "MasterVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SfxVolume";

        private void Awake() => Load();

        public void SetMasterVolume(int volume) => audioMixer.SetFloat(MASTER_VOLUME, volume);

        public void SetMusicVolume(int volume) => audioMixer.SetFloat(MUSIC_VOLUME, volume);

        public void SetSfxVolume(int volume) => audioMixer.SetFloat(SFX_VOLUME, volume);

        private void Load()
        {
            var masterVolume = PlayerPrefs.GetInt(MASTER_VOLUME, -50);
            var musicVolume = PlayerPrefs.GetInt(MUSIC_VOLUME, -50);
            var sfxVolume = PlayerPrefs.GetInt(SFX_VOLUME, -50);
            OnSettingsLoaded?.Invoke(masterVolume, musicVolume, sfxVolume);
        }

        private void OnApplicationQuit() => Save();

        private void Save()
        {
            audioMixer.GetFloat(MASTER_VOLUME, out var masterVolume);
            PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolume);
            audioMixer.GetFloat(MUSIC_VOLUME, out var musicVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
            audioMixer.GetFloat(SFX_VOLUME, out var sfxVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
            PlayerPrefs.Save();
        }
    }
}