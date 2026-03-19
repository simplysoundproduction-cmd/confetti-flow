using System;
using UnityEngine;

namespace ConfettiFlow.Core
{
    /// <summary>
    /// Central static event hub for all decoupled communication across systems.
    /// Subscribe/unsubscribe in OnEnable/OnDisable to avoid memory leaks.
    /// </summary>
    public static class GameEvents
    {
        // ── Game State ───────────────────────────────────────────────────────────
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameReset;

        // ── Balloon ──────────────────────────────────────────────────────────────
        /// <summary>Fired when a balloon is popped. Carries world position and balloon color.</summary>
        public static event Action<Vector2, Color> OnBalloonPopped;

        // ── Confetti ─────────────────────────────────────────────────────────────
        /// <summary>Fired when a confetti burst is requested at a world position with a given color.</summary>
        public static event Action<Vector2, Color> OnConfettiBurstRequested;

        // ── Color Fill ───────────────────────────────────────────────────────────
        /// <summary>Fired when the fill level changes. Value is 0–1 normalized.</summary>
        public static event Action<float> OnFillLevelChanged;
        /// <summary>Fired when fill reaches or exceeds the threshold to unlock next scene.</summary>
        public static event Action OnFillThresholdReached;

        // ── Scene / Flow ─────────────────────────────────────────────────────────
        public static event Action OnContinueRequested;
        public static event Action OnNextLevelRequested;

        // ── Dispatchers (call these to fire events safely) ───────────────────────

        public static void RaiseGamePaused()               => OnGamePaused?.Invoke();
        public static void RaiseGameResumed()              => OnGameResumed?.Invoke();
        public static void RaiseGameReset()                => OnGameReset?.Invoke();

        public static void RaiseBalloonPopped(Vector2 pos, Color color)
            => OnBalloonPopped?.Invoke(pos, color);

        public static void RaiseConfettiBurstRequested(Vector2 pos, Color color)
            => OnConfettiBurstRequested?.Invoke(pos, color);

        public static void RaiseFillLevelChanged(float normalizedValue)
            => OnFillLevelChanged?.Invoke(normalizedValue);

        public static void RaiseFillThresholdReached()     => OnFillThresholdReached?.Invoke();

        public static void RaiseContinueRequested()        => OnContinueRequested?.Invoke();
        public static void RaiseNextLevelRequested()       => OnNextLevelRequested?.Invoke();

        /// <summary>
        /// Call this on scene unload to clear all listeners and prevent stale references.
        /// </summary>
        public static void ClearAllListeners()
        {
            OnGamePaused              = null;
            OnGameResumed             = null;
            OnGameReset               = null;
            OnBalloonPopped           = null;
            OnConfettiBurstRequested  = null;
            OnFillLevelChanged        = null;
            OnFillThresholdReached    = null;
            OnContinueRequested       = null;
            OnNextLevelRequested      = null;
        }
    }
}
