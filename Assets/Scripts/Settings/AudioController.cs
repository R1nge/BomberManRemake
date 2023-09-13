using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings
{
    public class AudioController : MonoBehaviour
    {
        public event Action<float, float, float> OnSettingsLoaded;
        [SerializeField] private AudioMixer audioMixer;
        private const string MASTER_VOLUME = "MasterVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SfxVolume";

        private void Awake() => Load();

        public void SetMasterVolume(float volume) => audioMixer.SetFloat(MASTER_VOLUME, volume);

        public void SetMusicVolume(float volume) => audioMixer.SetFloat(MUSIC_VOLUME, volume);

        public void SetSfxVolume(float volume) => audioMixer.SetFloat(SFX_VOLUME, volume);

        private void Load()
        {
            var masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, -50f);
            var musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, -50f);
            var sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, -50f);
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