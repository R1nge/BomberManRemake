using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerCameraTPS : NetworkBehaviour
    {
        [SerializeField] private new GameObject camera;

        public override void OnNetworkSpawn()
        {
            camera.SetActive(IsOwner);
            camera.GetComponent<AudioListener>().enabled = IsOwner;
        }
    }
}