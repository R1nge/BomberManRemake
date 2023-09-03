using System;
using UnityEngine;

namespace Misc
{
    public class LockCursor : MonoBehaviour
    {
        [SerializeField] private CursorLockMode mode;

        private void Awake()
        {
            Cursor.lockState = mode;
            switch (mode)
            {
                case CursorLockMode.None:
                    Cursor.visible = true;
                    break;
                case CursorLockMode.Locked:
                    Cursor.visible = false;
                    break;
                case CursorLockMode.Confined:
                    Cursor.visible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}