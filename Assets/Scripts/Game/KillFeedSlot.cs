using TMPro;
using UnityEngine;

namespace Game
{
    public class KillFeedSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void Init(string killedName, string killerName)
        {
            if (killedName == killerName)
            {
                text.text = $"{killedName} suicided";
            }
            else
            {
                text.text = $"{killerName} killed {killedName}";
            }
        }
    }
}