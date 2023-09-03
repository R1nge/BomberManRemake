using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        private TestSingleton _test;

        [Inject]
        private void Inject(TestSingleton testSingleton)
        {
            _test = testSingleton;
        }

        private void Start()
        {
            if (!IsOwner) return;
            TestServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void TestServerRpc()
        {
            var number = _test.GetNumber();
            Debug.Log($"Server: {number}");

            var targetClient = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new List<ulong>(1)
                    {
                        1
                    }
                }
            };

            TestClientRpc(number, targetClient);
        }

        [ClientRpc]
        private void TestClientRpc(int number, ClientRpcParams rpcParams)
        {
            Debug.Log($"Server to Client: {number}");
        }
    }
}