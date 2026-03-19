using System.Collections.Generic;
using UnityEngine;
using ConfettiFlow.Core;

namespace ConfettiFlow.Confetti
{
    /// <summary>
    /// Pre-allocates a pool of ConfettiParticle objects and manages burst spawning.
    /// Listens to GameEvents.OnConfettiBurstRequested and dispenses pieces from the pool.
    /// Attach to the ConfettiPool GameObject in Gameplay hierarchy.
    /// </summary>
    public class ConfettiPool : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private ConfettiConfig config;
        [SerializeField] private GameObject confettiPiecePrefab;
        [SerializeField] private Transform confettiContainer;

        private readonly List<ConfettiParticle> _pool = new List<ConfettiParticle>();
        private bool _isConfigured;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _isConfigured = ValidateReferences();

            if (_isConfigured)
                PreWarm();
        }

        private void OnEnable()
        {
            GameEvents.OnConfettiBurstRequested += HandleBurstRequested;
            GameEvents.OnGameReset              += HandleReset;
        }

        private void OnDisable()
        {
            GameEvents.OnConfettiBurstRequested -= HandleBurstRequested;
            GameEvents.OnGameReset              -= HandleReset;
        }

        // ── Pool ─────────────────────────────────────────────────────────────────

        private void PreWarm()
        {
            if (!_isConfigured) return;

            for (int i = 0; i < config.poolSize; i++)
            {
                var piece = CreatePiece();
                piece.gameObject.SetActive(false);
            }
        }

        private ConfettiParticle CreatePiece()
        {
            var go     = Instantiate(confettiPiecePrefab, confettiContainer);
            var piece  = go.GetComponent<ConfettiParticle>();

            if (piece == null)
            {
                Debug.LogError("ConfettiPool: The assigned Confetti Piece Prefab is missing the ConfettiParticle component.", this);
                Destroy(go);
                return null;
            }

            _pool.Add(piece);
            return piece;
        }

        private ConfettiParticle GetFromPool()
        {
            if (!_isConfigured) return null;

            foreach (var p in _pool)
            {
                if (!p.gameObject.activeInHierarchy) return p;
            }
            // Pool exhausted — create an extra piece (shouldn't happen often)
            return CreatePiece();
        }

        // ── Burst ────────────────────────────────────────────────────────────────

        private void HandleBurstRequested(Vector2 position, Color color)
        {
            if (!_isConfigured) return;

            for (int i = 0; i < config.burstCount; i++)
            {
                var piece = GetFromPool();
                if (piece == null) return;
                piece.Activate(position, color, config);
            }
        }

        // ── Reset ────────────────────────────────────────────────────────────────

        private void HandleReset()
        {
            foreach (var p in _pool)
            {
                if (p != null)
                    p.gameObject.SetActive(false);
            }
        }

        // ── Public (used by ConfettiInteractionController) ───────────────────────

        /// <summary>Returns all currently active confetti particles for interaction.</summary>
        public List<ConfettiParticle> GetActiveParticles()
        {
            var active = new List<ConfettiParticle>();
            foreach (var p in _pool)
            {
                if (p != null && p.gameObject.activeInHierarchy) active.Add(p);
            }
            return active;
        }

        private bool ValidateReferences()
        {
            if (config == null)
            {
                Debug.LogWarning("ConfettiPool: Assign a ConfettiConfig asset in the Inspector.", this);
                return false;
            }

            if (confettiPiecePrefab == null)
            {
                Debug.LogWarning("ConfettiPool: Assign a Confetti Piece Prefab in the Inspector.", this);
                return false;
            }

            if (confettiContainer == null)
            {
                Debug.LogWarning("ConfettiPool: Assign a Confetti Container transform in the Inspector.", this);
                return false;
            }

            return true;
        }
    }
}
