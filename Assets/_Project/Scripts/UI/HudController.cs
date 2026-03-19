using UnityEngine;
using UnityEngine.UI;
using ConfettiFlow.Core;

namespace ConfettiFlow.UI
{
    /// <summary>
    /// Manages all minimal prototype HUD elements: Pause, Reset, Continue, Next,
    /// and fill progress bar. Wires buttons to GameEvents and GameStateController.
    /// Attach to a HudController GameObject that is a child of the Canvas.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button nextButton;

        [Header("Fill Progress")]
        [Tooltip("Slider or Image fill used to reflect the fill level (optional).")]
        [SerializeField] private Slider fillProgressSlider;

        [Header("Progression Visibility")]
        [Tooltip("Fallback: show Continue/Next when fill reaches this normalized value.")]
        [Range(0f, 1f)]
        [SerializeField] private float revealButtonsAtFill = 0.8f;

        [Header("Panel — shown when threshold reached")]
        [SerializeField] private GameObject nextLevelPanel;

        private bool _progressionVisible;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            AutoWireIfNeeded();
        }

        private void Start()
        {
            pauseButton?.onClick.RemoveListener(OnPauseClicked);
            resetButton?.onClick.RemoveListener(OnResetClicked);
            continueButton?.onClick.RemoveListener(OnContinueClicked);
            nextButton?.onClick.RemoveListener(OnNextClicked);

            pauseButton?.onClick.AddListener(OnPauseClicked);
            resetButton?.onClick.AddListener(OnResetClicked);
            continueButton?.onClick.AddListener(OnContinueClicked);
            nextButton?.onClick.AddListener(OnNextClicked);

            SetNextLevelPanelVisible(false);
            _progressionVisible = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                AutoWireIfNeeded();
        }
#endif

        private void OnEnable()
        {
            GameEvents.OnFillLevelChanged    += HandleFillLevelChanged;
            GameEvents.OnFillThresholdReached += HandleThresholdReached;
            GameEvents.OnGamePaused          += HandlePaused;
            GameEvents.OnGameResumed         += HandleResumed;
            GameEvents.OnGameReset           += HandleReset;
        }

        private void OnDisable()
        {
            GameEvents.OnFillLevelChanged    -= HandleFillLevelChanged;
            GameEvents.OnFillThresholdReached -= HandleThresholdReached;
            GameEvents.OnGamePaused          -= HandlePaused;
            GameEvents.OnGameResumed         -= HandleResumed;
            GameEvents.OnGameReset           -= HandleReset;
        }

        // ── Button Callbacks ─────────────────────────────────────────────────────

        private void OnPauseClicked()
        {
            var state = GameStateController.Instance;
            if (state.CurrentState == GameStateController.GameState.Paused)
                state.Resume();
            else
                state.Pause();
        }

        private void OnResetClicked()
        {
            GameStateController.Instance.Reset();
        }

        private void OnContinueClicked()
        {
            GameEvents.RaiseContinueRequested();
            GameStateController.Instance.Resume();
        }

        private void OnNextClicked()
        {
            GameEvents.RaiseNextLevelRequested();
        }

        // ── Event Handlers ───────────────────────────────────────────────────────

        private void HandleFillLevelChanged(float normalizedValue)
        {
            if (fillProgressSlider != null)
                fillProgressSlider.value = normalizedValue;

            if (!_progressionVisible && normalizedValue >= revealButtonsAtFill)
            {
                _progressionVisible = true;
                SetNextLevelPanelVisible(true);
            }
        }

        private void HandleThresholdReached()
        {
            _progressionVisible = true;
            SetNextLevelPanelVisible(true);
        }

        private void HandlePaused()
        {
            // Optionally swap pause button icon to a "resume" icon here
        }

        private void HandleResumed()
        {
            // Optionally restore pause button icon here
        }

        private void HandleReset()
        {
            _progressionVisible = false;
            SetNextLevelPanelVisible(false);
            if (fillProgressSlider != null)
                fillProgressSlider.value = 0f;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private void SetNextLevelPanelVisible(bool visible)
        {
            if (nextLevelPanel != null)
                nextLevelPanel.SetActive(visible);

            continueButton?.gameObject.SetActive(visible);
            nextButton?.gameObject.SetActive(visible);
        }

        private void AutoWireIfNeeded()
        {
            if (pauseButton == null) pauseButton = FindButtonByName("PauseButton");
            if (resetButton == null) resetButton = FindButtonByName("ResetButton");
            if (continueButton == null) continueButton = FindButtonByName("ContinueButton");
            if (nextButton == null) nextButton = FindButtonByName("NextButton");

            if (fillProgressSlider == null)
                fillProgressSlider = FindSliderByName("FillProgressBar");

            if (nextLevelPanel == null)
            {
                Transform panel = FindTransformByName("NextLevelPanel");
                if (panel != null) nextLevelPanel = panel.gameObject;
            }
        }

        private Button FindButtonByName(string objectName)
        {
            var root = GetSearchRoot();
            var buttons = root.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button.name == objectName)
                    return button;
            }
            return null;
        }

        private Slider FindSliderByName(string objectName)
        {
            var root = GetSearchRoot();
            var sliders = root.GetComponentsInChildren<Slider>(true);
            foreach (var slider in sliders)
            {
                if (slider.name == objectName)
                    return slider;
            }
            return null;
        }

        private Transform FindTransformByName(string objectName)
        {
            var root = GetSearchRoot();
            return FindTransformRecursive(root, objectName);
        }

        private Transform FindTransformRecursive(Transform current, string objectName)
        {
            if (current.name == objectName)
                return current;

            for (int i = 0; i < current.childCount; i++)
            {
                var result = FindTransformRecursive(current.GetChild(i), objectName);
                if (result != null)
                    return result;
            }

            return null;
        }

        private Transform GetSearchRoot()
        {
            var canvas = GetComponentInParent<Canvas>();
            return canvas != null ? canvas.transform : transform.root;
        }
    }
}
