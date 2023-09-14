using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenu, settings;
    private const string IP_INPUT_FIELD = "Ip";
    private const string NICK_INPUT_FIELD = "Nickname";
    private const string HOST_BUTTON = "Host";
    private const string JOIN_BUTTON = "Join";
    private const string SKINS_BUTTON = "Skins";
    private const string BOMBS_BUTTON = "Bombs";
    private const string SETTINGS_BUTTON = "Settings";

    private const string SETTINGS_BACKGROUND = "Background";

    private void OnEnable()
    {
        var root = mainMenu.rootVisualElement;

        root.Q<TextField>(IP_INPUT_FIELD).RegisterValueChangedCallback(evt =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = evt.newValue;
        });

        root.Q<TextField>(NICK_INPUT_FIELD).RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetString("Nick", evt.newValue);
            PlayerPrefs.Save();
        });

        root.Q<Button>(HOST_BUTTON).clicked += () =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyDataSingleton", LoadSceneMode.Additive);
        };

        root.Q<Button>(JOIN_BUTTON).clicked += () =>
        {
            NetworkManager.Singleton.StartClient();
        };

        root.Q<Button>(SETTINGS_BUTTON).clicked += () =>
        {
            root.visible = false;
            settings.rootVisualElement.Q<VisualElement>(SETTINGS_BACKGROUND).visible = true;
        };
    }
}