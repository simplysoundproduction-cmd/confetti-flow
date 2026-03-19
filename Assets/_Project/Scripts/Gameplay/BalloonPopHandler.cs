using UnityEngine;
using ConfettiFlow.Core;

namespace ConfettiFlow.Gameplay
{
    /// <summary>
    /// Detects a tap/click on this balloon's collider and fires the pop events
    /// that trigger confetti burst and color fill contribution.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(BalloonController))]
    public class BalloonPopHandler : MonoBehaviour
    {
        private BalloonController _controller;

        private void Awake()
        {
            _controller = GetComponent<BalloonController>();
        }

        // OnMouseDown works for both mouse (editor) and single-touch (mobile).
        // For multi-touch, TouchInputController raises taps and we use OnTapped().
        private void OnMouseDown()
        {
            Pop();
        }

        /// <summary>
        /// Called by TouchInputController when it detects a tap landing on this collider.
        /// Provides an alternative to OnMouseDown for proper multi-touch support.
        /// </summary>
        public void OnTapped()
        {
            Pop();
        }

        private void Pop()
        {
            if (!gameObject.activeInHierarchy) return;
            if (GameStateController.Instance != null && !GameStateController.Instance.IsPlaying) return;

            Vector2 worldPos = transform.position;
            Color color      = _controller.BalloonColor;

            // 1. Notify confetti system
            GameEvents.RaiseConfettiBurstRequested(worldPos, color);

            // 2. Notify color fill system
            GameEvents.RaiseBalloonPopped(worldPos, color);

            // 3. Deactivate the balloon
            _controller.Deactivate();
        }
    }
}
