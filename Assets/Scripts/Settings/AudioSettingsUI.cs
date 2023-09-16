using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private GameObject settings;
        [SerializeField] private Slider master, music, sfx;
        [SerializeField] private Button open, close;
        private AudioController _audioController;

        private void Awake()
        {
            _audioController = GetComponent<AudioController>();
            _audioController.OnSettingsLoaded += SettingsLoaded;

            master.onValueChanged.AddListener(volume => _audioController.SetMasterVolume(volume));
            music.onValueChanged.AddListener(volume => _audioController.SetMusicVolume(volume));
            sfx.onValueChanged.AddListener(volume => _audioController.SetSfxVolume(volume));

            open.onClick.AddListener(Open);
            close.onClick.AddListener(Close);
        }

        private void Open() => settings.SetActive(true);

        private void Close() => settings.SetActive(false);

        private void SettingsLoaded(float masterValue, float musicValue, float sfxValue)
        {
            master.value = masterValue;
            music.value = musicValue;
            sfx.value = sfxValue;
        }

        private void OnDestroy() => _audioController.OnSettingsLoaded -= SettingsLoaded;
    }
}