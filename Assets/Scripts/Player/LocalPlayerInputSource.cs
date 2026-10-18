using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace BizimKosemiz.Player
{
    /// <summary>
    /// Oyuncu hareket (WASD / Sol Analoğu), koşma (Shift) ve kamera bakış (Mouse / Sağ Analoğu)
    /// girdilerini okur. Yalnızca yerel oyuncu için etkinleştirilir.
    /// </summary>
    [DisallowMultipleComponent]
    public class LocalPlayerInputSource : MonoBehaviour
    {
        [SerializeField] private bool lockCursorInPlayMode = false;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprintPressed { get; private set; }

        private void OnEnable()
        {
            if (lockCursorInPlayMode && Application.isPlaying)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void ReadInputs(bool isLocalOwner)
        {
            if (!isLocalOwner)
            {
                MoveInput = Vector2.zero;
                LookInput = Vector2.zero;
                IsSprintPressed = false;
                return;
            }

            Vector2 move = Vector2.zero;
            Vector2 look = Vector2.zero;
            bool sprint = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                float x = 0f;
                float y = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
                move = new Vector2(x, y);
                sprint = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }

            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.sqrMagnitude > move.sqrMagnitude)
                {
                    move = stick;
                }
                sprint |= Gamepad.current.leftShoulder.isPressed || Gamepad.current.leftStickButton.isPressed;
            }

            if (Mouse.current != null)
            {
                // Sağ tık basılıyken veya imleç kilitliyken kamerayı döndür
                if (Cursor.lockState == CursorLockMode.Locked || Mouse.current.rightButton.isPressed)
                {
                    look = Mouse.current.delta.ReadValue() * 0.12f;
                }
            }
#else
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (Cursor.lockState == CursorLockMode.Locked || Input.GetMouseButton(1))
            {
                look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 2.5f;
            }
#endif

            if (move.sqrMagnitude > 1f)
            {
                move.Normalize();
            }

            MoveInput = move;
            LookInput = look;
            IsSprintPressed = sprint;
        }
    }
}
