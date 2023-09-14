using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Settings
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private UIDocument settings, mainMenu;
        private AudioController _audioController;
        private const string MASTER_VOLUME = "Master";
        private const string MUSIC_VOLUME = "Music";
        private const string SFX_VOLUME = "Sfx";
        private const string CLOSE_BUTTON = "Close";

        private const string BACKGROUND = "Background";

        private void OnEnable()
        {
            var root = settings.rootVisualElement;

            root.Q<SliderInt>(MASTER_VOLUME).RegisterValueChangedCallback(evt =>
            {
                _audioController.SetMasterVolume(evt.newValue);
            });

            root.Q<SliderInt>(MUSIC_VOLUME).RegisterValueChangedCallback(evt =>
            {
                _audioController.SetMusicVolume(evt.newValue);
            });

            root.Q<SliderInt>(SFX_VOLUME).RegisterValueChangedCallback(evt =>
            {
                _audioController.SetSfxVolume(evt.newValue);
            });

            root.Q<Button>(CLOSE_BUTTON).clicked += () =>
            {
                root.Q<VisualElement>(BACKGROUND).visible = false;
                mainMenu.rootVisualElement.visible = true;
            };

            root.Q<VisualElement>(BACKGROUND).visible = false;
        }

        private void Awake()
        {
            _audioController = GetComponent<AudioController>();
            _audioController.OnSettingsLoaded += SettingsLoaded;
        }

        private void SettingsLoaded(int masterValue, int musicValue, int sfxValue)
        {
            var root = settings.rootVisualElement;

            root.Q<SliderInt>(MASTER_VOLUME).value = masterValue;
            root.Q<SliderInt>(MUSIC_VOLUME).value = musicValue;
            root.Q<SliderInt>(SFX_VOLUME).value = sfxValue;
        }

        private void OnDestroy() => _audioController.OnSettingsLoaded -= SettingsLoaded;
    }
}