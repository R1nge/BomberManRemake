using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerFpsCamera : NetworkBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Rotate;
        }

        private void Start()
        {
            camera.enabled = IsOwner;
            camera.GetComponent<AudioListener>().enabled = IsOwner;
        }

        private void Rotate()
        {
            if (!IsOwner) return;
            _rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            //RotateServerRpc(Input.GetAxis("Mouse X") * sensitivity);
        }

        [ServerRpc]
        private void RotateServerRpc(float rotationX)
        {
            transform.rotation *= Quaternion.Euler(0, rotationX, 0);
        }

        public override void OnDestroy()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick -= Rotate;
        }
    }
}