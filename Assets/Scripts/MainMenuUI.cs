using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nick;
    [SerializeField] private Button host, join;

    private void Awake()
    {
        nick.onEndEdit.AddListener(s =>
        {
            PlayerPrefs.SetString("Nick", s);
            PlayerPrefs.Save();
        });
        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });

        join.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
    }
}