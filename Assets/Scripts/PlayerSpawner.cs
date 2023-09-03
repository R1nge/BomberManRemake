using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private DiContainer _diContainer;

    [Inject]
    private void Inject(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SingletonOnOnClientConnectedCallback;
    }

    private void SingletonOnOnClientConnectedCallback(ulong obj)
    {
        if (obj == 0) return;
        SpawnServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc()
    {
        var player = _diContainer.InstantiatePrefab(playerPrefab);
        player.GetComponent<NetworkObject>().Spawn();
    }
}