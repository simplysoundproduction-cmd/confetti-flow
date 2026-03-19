using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ConfettiFlow.Core;
using ConfettiFlow.Gameplay;

namespace ConfettiFlow.Input
{
    /// <summary>
    /// Reads touch/pointer input via the new Input System.
    /// Raycasts taps onto balloons to trigger pops, and broadcasts
    /// swipe deltas to ConfettiInteractionController.
    /// Attach to the TouchInputController GameObject in Input hierarchy.
    /// </summary>
    public class TouchInputController : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask balloonLayer;

        // Track pointer positions per touchId for swipe delta
        private readonly Dictionary<int, Vector2> _lastPositions = new Dictionary<int, Vector2>();

        // Broadcast swipe delta to any listener (ConfettiInteractionController)
        public static event System.Action<Vector2, Vector2> OnSwipeDelta;
        // world pos, delta

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Update()
        {
            if (GameStateController.Instance != null && !GameStateController.Instance.IsPlaying) return;

            var activeTouches = Touchscreen.current;

            if (activeTouches != null)
            {
                foreach (var touch in activeTouches.touches)
                {
                    ProcessTouch(touch);
                }
            }
            else if (Mouse.current != null)
            {
                // Editor fallback
                ProcessMouse();
            }
        }

        // ── Touch ────────────────────────────────────────────────────────────────

        private void ProcessTouch(UnityEngine.InputSystem.Controls.TouchControl touch)
        {
            int id = touch.touchId.ReadValue();
            var phase = touch.phase.ReadValue();
            Vector2 screenPos = touch.position.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _lastPositions[id] = screenPos;
                TryPopBalloon(screenPos);
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                if (_lastPositions.TryGetValue(id, out Vector2 last))
                {
                    Vector2 worldPos   = ScreenToWorld(screenPos);
                    Vector2 prevWorld  = ScreenToWorld(last);
                    Vector2 delta      = worldPos - prevWorld;
                    OnSwipeDelta?.Invoke(worldPos, delta);
                    _lastPositions[id] = screenPos;
                }
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                _lastPositions.Remove(id);
            }
        }

        private void ProcessMouse()
        {
            var mouse = Mouse.current;
            Vector2 screenPos = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _lastPositions[0] = screenPos;
                TryPopBalloon(screenPos);
            }
            else if (mouse.leftButton.isPressed)
            {
                if (_lastPositions.TryGetValue(0, out Vector2 last))
                {
                    Vector2 worldPos  = ScreenToWorld(screenPos);
                    Vector2 prevWorld = ScreenToWorld(last);
                    Vector2 delta     = worldPos - prevWorld;
                    if (delta.sqrMagnitude > 0.0001f)
                        OnSwipeDelta?.Invoke(worldPos, delta);
                    _lastPositions[0] = screenPos;
                }
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                _lastPositions.Remove(0);
            }
        }

        // ── Balloon Raycast ──────────────────────────────────────────────────────

        private void TryPopBalloon(Vector2 screenPos)
        {
            Vector2 world = ScreenToWorld(screenPos);
            var hit = Physics2D.OverlapPoint(world, balloonLayer);

            if (hit != null)
            {
                var popHandler = hit.GetComponent<BalloonPopHandler>();
                popHandler?.OnTapped();
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private Vector2 ScreenToWorld(Vector2 screenPos)
            => mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane));
    }
}
