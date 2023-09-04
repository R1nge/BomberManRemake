using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class BombSound : NetworkBehaviour
    {
        [SerializeField] private NetworkObject soundPrefab;
        private BombTimer _bombTimer;

        private void Awake()
        {
            _bombTimer = GetComponent<BombTimer>();
            _bombTimer.OnTimeRunOut += SpawnSoundServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnSoundServerRpc()
        {
            var sound = Instantiate(soundPrefab, transform.position, Quaternion.identity);
            sound.Spawn();
        }

        public override void OnDestroy() => _bombTimer.OnTimeRunOut -= SpawnSoundServerRpc;
    }
}