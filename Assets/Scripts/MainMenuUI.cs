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

    private void Awake()
    {
        ip.onEndEdit.AddListener(s =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = s;
        });

        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });

        join.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });

        quit.onClick.AddListener(Application.Quit);
    }

    private void OnDestroy()
    {
        host.onClick.RemoveAllListeners();
        join.onClick.RemoveAllListeners();
        quit.onClick.RemoveAllListeners();
    }
}