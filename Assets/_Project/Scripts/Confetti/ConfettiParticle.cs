using UnityEngine;

namespace ConfettiFlow.Confetti
{
    /// <summary>
    /// Controls a single confetti piece: burst velocity, gravity, drag, rotation,
    /// lifetime, tilt response, and swipe interaction.
    /// Managed entirely by ConfettiPool — no direct scene references needed.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ConfettiParticle : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private ConfettiConfig _config;

        private Vector2 _velocity;
        private float   _rotationSpeed;
        private float   _lifetime;
        private float   _age;
        private bool    _isActive;

        // Cached tilt input set each frame by TiltInputController
        private static Vector2 s_tiltInput;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!_isActive) return;

            // Age & lifetime
            _age += Time.deltaTime;
            if (_age >= _lifetime)
            {
                gameObject.SetActive(false);
                _isActive = false;
                return;
            }

            // Gravity
            _velocity.y -= _config.gravity * Time.deltaTime;

            // Tilt
            _velocity.x += s_tiltInput.x * _config.tiltInfluence * Time.deltaTime;

            // Drag (proportional, frame-rate-independent approx)
            _velocity *= (1f - _config.drag * 60f * Time.deltaTime);

            // Move
            transform.position += (Vector3)(_velocity * Time.deltaTime);

            // Rotate
            transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>Activates and initializes this piece for a new burst.</summary>
        public void Activate(Vector2 startPos, Color color, ConfettiConfig config)
        {
            _config = config;
            transform.position = startPos;

            float speed  = Random.Range(config.minBurstSpeed, config.maxBurstSpeed);
            float angle  = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _velocity    = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            _rotationSpeed  = Random.Range(-config.maxRotationSpeed, config.maxRotationSpeed);
            _lifetime       = Random.Range(config.minLifetime, config.maxLifetime);
            _age            = 0f;

            float size = Random.Range(config.minSize, config.maxSize);
            transform.localScale = Vector3.one * size;

            // Slight color variation for a richer burst
            _sr.color = Random.ColorHSV(
                Mathf.Clamp01(RgbToHue(color) - 0.05f),
                Mathf.Clamp01(RgbToHue(color) + 0.05f),
                0.7f, 1f,
                0.8f, 1f
            );

            _isActive = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Called by ConfettiInteractionController when a swipe passes nearby.
        /// Applies an impulse toward the swipe direction.
        /// </summary>
        public void ApplySwipeForce(Vector2 force)
        {
            _velocity += force;
        }

        /// <summary>Set by TiltInputController each frame for all ConfettiParticles.</summary>
        public static void SetTiltInput(Vector2 tilt)
        {
            s_tiltInput = tilt;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private float RgbToHue(Color c)
        {
            Color.RGBToHSV(c, out float h, out _, out _);
            return h;
        }
    }
}
