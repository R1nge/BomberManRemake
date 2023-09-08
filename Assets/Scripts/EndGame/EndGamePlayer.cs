using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace EndGame
{
    public class EndGamePlayer : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI scoreText;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(Lobby.Lobby lobby)
        {
            _lobby = lobby;
        }

        public void UpdateNick(ulong clientId)
        {
            var data = _lobby.GetData(clientId);
            if (data != null)
            {
                UpdateNickClientRpc(data.Value.NickName);
            }
        }

        [ClientRpc]
        private void UpdateNickClientRpc(string nick)
        {
            nickname.text = nick;
        }

        public void UpdateScore(ulong clientId)
        {
            var data = _lobby.GetData(clientId);
            if (data != null)
            {
                UpdateScoreClientRpc(data.Value.Points);
            }
        }

        [ClientRpc]
        private void UpdateScoreClientRpc(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}