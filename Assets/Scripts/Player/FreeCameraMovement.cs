using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class FreeCameraMovement : MonoBehaviour
    {
        [SerializeField] private Transform camera;
        [SerializeField] private float speed;
        private float _speedX, _speedZ;

        private void Update()
        {
            var direction = new Vector3(_speedX, 0, _speedZ);
            transform.Translate(camera.localRotation * direction * Time.deltaTime);
        }
        
        public void OnMove(InputValue value)
        {
            _speedX = value.Get<Vector2>().x * speed;
            _speedZ = value.Get<Vector2>().y * speed;
        }
    }
}