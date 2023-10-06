using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ip;
    [SerializeField] private Button host, join;
    [SerializeField] private Button quit;

    private void Start()
    {
        ip.onEndEdit.AddListener(SetIp);

        host.onClick.AddListener(Host);

        join.onClick.AddListener(StartClient);

        quit.onClick.AddListener(Application.Quit);
    }

    private void SetIp(string s)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = s;
    }

    private void Host()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnDestroy()
    {
        host.onClick.RemoveAllListeners();
        join.onClick.RemoveAllListeners();
        quit.onClick.RemoveAllListeners();
    }
}