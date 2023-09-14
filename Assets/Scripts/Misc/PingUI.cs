using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Misc
{
    public class PingUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI ping;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += UpdateUI;
        }

        public override void OnNetworkSpawn()
        {
            ping.gameObject.SetActive(IsOwner);
        }

        private void UpdateUI()
        {
            if (!IsOwner) return;
            var pin = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);
            pin /= 2;
            ping.text = $"{pin} ms";
        }
    }
}