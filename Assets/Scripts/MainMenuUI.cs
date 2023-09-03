using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button host, join;

    private void Awake()
    {
        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });
        
        join.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
    }
}