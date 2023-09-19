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
    }

    private void OnDestroy()
    {
        host.onClick.RemoveAllListeners();
        join.onClick.RemoveAllListeners();
    }
}