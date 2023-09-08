using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class MobileControls : NetworkBehaviour
    {
        [SerializeField] private GameObject leftStick, rightStick;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
#if !UNITY_ANDROID
                DisableMobileUI();
#endif
            }
            else
            {
                DisableMobileUI();
            }
        }

        private void DisableMobileUI()
        {
            leftStick.gameObject.SetActive(false);
            rightStick.gameObject.SetActive(false);
        }
    }
}