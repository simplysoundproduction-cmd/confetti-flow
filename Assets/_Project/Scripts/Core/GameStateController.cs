using UnityEngine;
using ConfettiFlow.Core;

namespace ConfettiFlow.Core
{
    /// <summary>
    /// Owns the current game state and drives pause/resume/reset transitions.
    /// Attach to the GameStateController GameObject in the Systems hierarchy.
    /// </summary>
    public class GameStateController : MonoBehaviour
    {
        public enum GameState
        {
            Playing,
            Paused,
            FillReached,   // fill threshold met — player can Continue or go Next
            Transitioning
        }

        public static GameStateController Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Playing;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnFillThresholdReached += HandleFillThresholdReached;
        }

        private void OnDisable()
        {
            GameEvents.OnFillThresholdReached -= HandleFillThresholdReached;
        }

        // ── Public API ───────────────────────────────────────────────────────────

        public void Pause()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            GameEvents.RaiseGamePaused();
        }

        public void Resume()
        {
            if (CurrentState != GameState.Paused && CurrentState != GameState.FillReached) return;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            GameEvents.RaiseGameResumed();
        }

        public void Reset()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            GameEvents.RaiseGameReset();
        }

        public bool IsPlaying => CurrentState == GameState.Playing;

        // ── Event Handlers ───────────────────────────────────────────────────────

        private void HandleFillThresholdReached()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.FillReached;
            // Do not pause time — let confetti keep drifting for sensory continuity
        }
    }
}
