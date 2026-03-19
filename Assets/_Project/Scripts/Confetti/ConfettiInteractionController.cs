using UnityEngine;
using ConfettiFlow.Confetti;
using ConfettiFlow.Input;

namespace ConfettiFlow.Confetti
{
    /// <summary>
    /// Listens to swipe deltas from TouchInputController and applies force
    /// to any ConfettiParticle within the swipe influence radius.
    /// Attach to ConfettiPool or a dedicated child GameObject.
    /// </summary>
    public class ConfettiInteractionController : MonoBehaviour
    {
        [SerializeField] private ConfettiPool confettiPool;
        [SerializeField] private ConfettiConfig config;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            TouchInputController.OnSwipeDelta += HandleSwipe;
        }

        private void OnDisable()
        {
            TouchInputController.OnSwipeDelta -= HandleSwipe;
        }

        // ── Swipe Handling ───────────────────────────────────────────────────────

        private void HandleSwipe(Vector2 worldPos, Vector2 delta)
        {
            float radiusSq = config.swipeInfluenceRadius * config.swipeInfluenceRadius;
            Vector2 forceDir = delta.normalized * config.swipeForceMagnitude;

            var particles = confettiPool.GetActiveParticles();
            foreach (var p in particles)
            {
                Vector2 toParticle = (Vector2)p.transform.position - worldPos;
                if (toParticle.sqrMagnitude <= radiusSq)
                {
                    // Scale force by inverse distance for a natural feel
                    float distFactor = 1f - (toParticle.magnitude / config.swipeInfluenceRadius);
                    p.ApplySwipeForce(forceDir * distFactor);
                }
            }
        }
    }
}
