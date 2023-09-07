using Game;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerFreeCamera : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX;
        private RoundManager _roundManager;

        [Inject]
        private void Inject(RoundManager roundManager) => _roundManager = roundManager;

        private void Awake() => _roundManager.OnCleanUpBeforeNextRound += Destroy;

        private void Destroy() => Destroy(gameObject);

        private void Update()
        {
            _rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }

        private void OnDestroy() => _roundManager.OnCleanUpBeforeNextRound -= Destroy;
    }
}