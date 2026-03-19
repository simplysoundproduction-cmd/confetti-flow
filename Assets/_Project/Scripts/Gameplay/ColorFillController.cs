using UnityEngine;
using ConfettiFlow.Core;

namespace ConfettiFlow.Gameplay
{
    /// <summary>
    /// Accumulates color contributions from popped balloons, maintains a normalized
    /// fill level, and fires OnFillThresholdReached when the target is met.
    /// Drives the BottomColorFillView visual through a MaterialPropertyBlock.
    /// Attach to ColorFillController GameObject in Gameplay hierarchy.
    /// </summary>
    public class ColorFillController : MonoBehaviour
    {
        [Header("Fill Settings")]
        [Tooltip("How much each balloon pop adds to the fill level.")]
        [Range(0.005f, 0.1f)]
        [SerializeField] private float fillAmountPerPop = 0.025f;

        [Tooltip("Normalized fill level (0–1) at which threshold is reached.")]
        [Range(0.1f, 1f)]
        [SerializeField] private float fillThreshold = 0.8f;

        [Header("Visual")]
        [Tooltip("The SpriteRenderer driving the color fill bar at the bottom.")]
        [SerializeField] private SpriteRenderer fillRenderer;

        [Tooltip("How smoothly the visual fill bar catches up to the target (units/sec).")]
        [SerializeField] private float fillSmoothSpeed = 1.2f;

        // ── State ────────────────────────────────────────────────────────────────
        private float _targetFill     = 0f;
        private float _displayFill    = 0f;
        private Color _currentColor   = Color.white;
        private bool  _thresholdFired = false;

        private MaterialPropertyBlock _mpb;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            UpdateFillVisual(0f);
        }

        private void OnEnable()
        {
            GameEvents.OnBalloonPopped += HandleBalloonPopped;
            GameEvents.OnGameReset     += HandleReset;
        }

        private void OnDisable()
        {
            GameEvents.OnBalloonPopped -= HandleBalloonPopped;
            GameEvents.OnGameReset     -= HandleReset;
        }

        private void Update()
        {
            if (Mathf.Approximately(_displayFill, _targetFill)) return;

            _displayFill = Mathf.MoveTowards(_displayFill, _targetFill, fillSmoothSpeed * Time.deltaTime);
            UpdateFillVisual(_displayFill);
            GameEvents.RaiseFillLevelChanged(_displayFill);
        }

        // ── Event Handlers ───────────────────────────────────────────────────────

        private void HandleBalloonPopped(Vector2 _, Color color)
        {
            _targetFill   = Mathf.Clamp01(_targetFill + fillAmountPerPop);
            _currentColor = Color.Lerp(_currentColor, color, 0.35f);

            if (!_thresholdFired && _targetFill >= fillThreshold)
            {
                _thresholdFired = true;
                GameEvents.RaiseFillThresholdReached();
            }
        }

        private void HandleReset()
        {
            _targetFill     = 0f;
            _displayFill    = 0f;
            _currentColor   = Color.white;
            _thresholdFired = false;
            UpdateFillVisual(0f);
        }

        // ── Visual ───────────────────────────────────────────────────────────────

        private void UpdateFillVisual(float fill)
        {
            if (fillRenderer == null) return;

            // Scale the fill bar vertically based on fill level
            Vector3 scale = fillRenderer.transform.localScale;
            scale.y = Mathf.Max(0.01f, fill);
            fillRenderer.transform.localScale = scale;

            // Update color via MaterialPropertyBlock (no material instance allocation)
            fillRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_Color", _currentColor);
            fillRenderer.SetPropertyBlock(_mpb);
        }
    }
}
