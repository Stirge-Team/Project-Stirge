using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Management
{
    public class CursorLockManager : MonoBehaviour
    {
        private void Start()
        {
            ChangeCursorLockMode(CursorLockMode.Locked);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                ChangeCursorLockMode(CursorLockMode.Locked);
            }
            else
            {
                ChangeCursorLockMode(CursorLockMode.None);
            }
        }

        private void ChangeCursorLockMode(CursorLockMode targetMode)
        {
            if (targetMode != Cursor.lockState)
            {
                Cursor.lockState = targetMode;
                switch (targetMode)
                {
                    case CursorLockMode.None:
                        Cursor.visible = true;
                        break;
                    case CursorLockMode.Locked:
                        Cursor.visible = false;
                        break;
                }
            }
        }

        public void OnEscapePressed(InputAction.CallbackContext context)
        {
            if (context.performed)
                ChangeCursorLockMode(CursorLockMode.None);
        }
    }
}
