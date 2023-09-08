using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ip, nick;
    [SerializeField] private Button host, join;

    private void Awake()
    {
        ip.onEndEdit.AddListener(s =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = s;
        });
        
        nick.onEndEdit.AddListener(s =>
        {
            PlayerPrefs.SetString("Nick", s);
            PlayerPrefs.Save();
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
        nick.onEndEdit.RemoveAllListeners();
        host.onClick.RemoveAllListeners();
        join.onClick.RemoveAllListeners();
    }
}