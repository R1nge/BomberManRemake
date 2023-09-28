using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ip;
    [SerializeField] private Button host, join;
    [SerializeField] private Button quit;
    private ExitGame _exitGame;

    [Inject]
    private void Inject(ExitGame exitGame) => _exitGame = exitGame;

    private void Awake()
    {
        ip.onEndEdit.AddListener(s =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = s;
        });

        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyDataSingleton", LoadSceneMode.Additive);
        });

        join.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });

        quit.onClick.AddListener(() => _exitGame.Exit());
    }

    private void OnDestroy()
    {
        host.onClick.RemoveAllListeners();
        join.onClick.RemoveAllListeners();
        quit.onClick.RemoveAllListeners();
    }
}