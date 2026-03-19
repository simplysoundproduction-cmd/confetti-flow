using UnityEngine;
using UnityEngine.InputSystem;
using ConfettiFlow.Confetti;
using ConfettiFlow.Core;

namespace ConfettiFlow.Input
{
    /// <summary>
    /// Reads device accelerometer/gyroscope tilt and feeds a normalized 2D vector
    /// to ConfettiParticle.SetTiltInput() each frame so confetti drifts with phone tilt.
    /// Attach to the TiltInputController GameObject in Input hierarchy.
    /// </summary>
    public class TiltInputController : MonoBehaviour
    {
        [Header("Sensitivity")]
        [Tooltip("Multiplier applied to raw accelerometer X axis.")]
        [Range(0.1f, 3f)]
        public float tiltSensitivity = 1.0f;

        [Tooltip("Deadzone — tilt below this magnitude is ignored.")]
        [Range(0f, 0.2f)]
        public float deadzone = 0.05f;

        [Tooltip("Smoothing factor (0=instant, 1=no movement).")]
        [Range(0f, 0.95f)]
        public float smoothing = 0.85f;

        private Vector2 _smoothedTilt = Vector2.zero;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            // Enable accelerometer if available
            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
        }

        private void OnDisable()
        {
            if (Accelerometer.current != null)
                InputSystem.DisableDevice(Accelerometer.current);
        }

        private void Update()
        {
            if (GameStateController.Instance != null && !GameStateController.Instance.IsPlaying)
            {
                ConfettiParticle.SetTiltInput(Vector2.zero);
                return;
            }

            Vector2 raw = GetTiltInput();

            // Deadzone
            if (raw.magnitude < deadzone) raw = Vector2.zero;

            // Smooth
            _smoothedTilt = Vector2.Lerp(raw, _smoothedTilt, smoothing);

            ConfettiParticle.SetTiltInput(_smoothedTilt * tiltSensitivity);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private Vector2 GetTiltInput()
        {
            if (Accelerometer.current != null)
            {
                var accel = Accelerometer.current.acceleration.ReadValue();
                // In portrait mode, X tilt is accel.x; Y tilt is accel.z (forward/back)
                return new Vector2(accel.x, accel.z);
            }

            // Editor fallback: arrow keys for testing
#if UNITY_EDITOR
            float x = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.isPressed)  x = -1f;
                if (Keyboard.current.rightArrowKey.isPressed) x =  1f;
            }
            return new Vector2(x, 0f);
#else
            return Vector2.zero;
#endif
        }
    }
}
