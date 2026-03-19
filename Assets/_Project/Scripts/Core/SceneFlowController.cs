using UnityEngine;
using UnityEngine.SceneManagement;
using ConfettiFlow.Core;

namespace ConfettiFlow.Core
{
    /// <summary>
    /// Handles top-level scene navigation: reload current scene on reset,
    /// and load the next scene when the player requests progression.
    /// Attach to the Systems GameBootstrap or a dedicated SceneFlowController object.
    /// </summary>
    public class SceneFlowController : MonoBehaviour
    {
        [Header("Scene Names")]
        [Tooltip("Name of the next sensory scene to transition to. Leave empty to loop current scene.")]
        [SerializeField] private string nextSceneName = "";

        [Tooltip("Duration of soft fade before scene transition (seconds). 0 = instant.")]
        [SerializeField] private float transitionDelay = 0.5f;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            GameEvents.OnGameReset         += HandleReset;
            GameEvents.OnNextLevelRequested += HandleNextLevel;
            GameEvents.OnContinueRequested  += HandleContinue;
        }

        private void OnDisable()
        {
            GameEvents.OnGameReset          -= HandleReset;
            GameEvents.OnNextLevelRequested -= HandleNextLevel;
            GameEvents.OnContinueRequested  -= HandleContinue;
        }

        // ── Event Handlers ───────────────────────────────────────────────────────

        private void HandleReset()
        {
            GameEvents.ClearAllListeners();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void HandleNextLevel()
        {
            string target = string.IsNullOrEmpty(nextSceneName)
                ? SceneManager.GetActiveScene().name   // loop current scene for now
                : nextSceneName;

            StartCoroutine(LoadWithDelay(target));
        }

        private void HandleContinue()
        {
            // Stay in current scene — GameStateController.Resume() already handles this.
            // Add fade-in or particle celebration here in future.
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private System.Collections.IEnumerator LoadWithDelay(string sceneName)
        {
            // Placeholder for future fade-out animation
            yield return new WaitForSecondsRealtime(transitionDelay);

            GameEvents.ClearAllListeners();
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
