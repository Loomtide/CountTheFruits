using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Verification
{
    /// <summary>
    /// Replay #1b fixture: moves horizontally while a key is held, reading the
    /// SIMULATED Input System keyboard (Keyboard.current) — the same device the
    /// bridge's virtual keyboard injects into. Deliberately NOT legacy Input.GetKey
    /// (the virtual device does not feed the old Input Manager).
    ///
    /// `CurrentX` is a PUBLIC serialized field updated every frame, so a replay can
    /// verify movement via runtime.assert_condition (auto-properties aren't readable;
    /// a public field is — the outcome-verification constraint).
    /// </summary>
    public class KeyboardMover : MonoBehaviour
    {
        [Tooltip("World units per second while a direction key is held.")]
        public float Speed = 3f;

        [Tooltip("Live X position — read by the replay assertion to prove movement.")]
        public float CurrentX;

        private void Start()
        {
            CurrentX = transform.position.x;
        }

        private void Update()
        {
            float dir = 0f;
#if ENABLE_INPUT_SYSTEM
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dir += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) dir -= 1f;
            }
#endif
            if (dir != 0f)
            {
                Vector3 p = transform.position;
                p.x += dir * Speed * Time.deltaTime;
                transform.position = p;
            }

            CurrentX = transform.position.x;
        }
    }
}
