using System;
using UnityEngine;

namespace ConfettiFlow.Gameplay
{
    /// <summary>
    /// Drives a single balloon's upward rise, horizontal sway, color assignment,
    /// and auto-despawn when it floats off the top of the screen.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BalloonController : MonoBehaviour
    {
        // Set by BalloonSpawner.Initialize()
        private BalloonSpawnConfig _config;
        private Color _color;
        private Action _onDeactivated;

        private SpriteRenderer _sr;
        private float _riseSpeed;
        private float _driftAmplitude;
        private float _driftOffset;     // random phase so each balloon sways differently
        private bool _isInitialized;

        // ── Public Color ─────────────────────────────────────────────────────────
        public Color BalloonColor => _color;

        // ── Setup ────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>Called by BalloonSpawner before SetActive(true).</summary>
        public void Initialize(BalloonSpawnConfig config, Color color, Action onDeactivated)
        {
            _config         = config;
            _color          = color;
            _onDeactivated  = onDeactivated;
            _riseSpeed      = UnityEngine.Random.Range(config.minRiseSpeed, config.maxRiseSpeed);
            _driftAmplitude = UnityEngine.Random.Range(0f, config.maxDriftAmplitude);
            _driftOffset    = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            _isInitialized  = true;

            if (_sr != null)
            {
                color.a = 1f;
                _sr.color = color;
                _sr.sortingOrder = 20;
            }
        }

        // ── Update ───────────────────────────────────────────────────────────────

        private void Update()
        {
            if (!_isInitialized) return;

            // Rise
            transform.position += Vector3.up * _riseSpeed * Time.deltaTime;

            // Sway
            float swayX = Mathf.Sin(Time.time * _config.driftFrequency + _driftOffset) * _driftAmplitude;
            Vector3 pos = transform.position;
            // Apply sway relative to vertical movement only; avoid double-accumulation
            pos.x += swayX * Time.deltaTime;
            transform.position = pos;

            // Auto-despawn
            if (transform.position.y >= _config.despawnYOffset)
                Deactivate();
        }

        // ── Public ───────────────────────────────────────────────────────────────

        /// <summary>Called by BalloonPopHandler when the balloon is tapped.</summary>
        public void Deactivate()
        {
            _isInitialized = false;
            gameObject.SetActive(false);
            _onDeactivated?.Invoke();
        }
    }
}
