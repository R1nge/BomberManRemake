using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class CountDownUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private CountDown _countDown;

        private void Awake()
        {
            _countDown = GetComponent<CountDown>();
            _countDown.TimeChanged += TimeChanged;
        }

        private void TimeChanged(float time)
        {
            timerText.text = time.ToString("#");
            if (time == 0)
            {
                timerText.text = "";
            }
        }

        private void OnDestroy() => _countDown.TimeChanged -= TimeChanged;
    }
}