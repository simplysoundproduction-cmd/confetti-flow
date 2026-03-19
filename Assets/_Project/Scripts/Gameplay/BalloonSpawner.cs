using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConfettiFlow.Core;

namespace ConfettiFlow.Gameplay
{
    /// <summary>
    /// Spawns balloons at timed intervals using a simple object pool.
    /// Respects the max-on-screen cap and game state (won't spawn while paused).
    /// Attach to the BalloonSpawner GameObject in Gameplay hierarchy.
    /// </summary>
    public class BalloonSpawner : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BalloonSpawnConfig config;
        [SerializeField] private GameObject balloonPrefab;
        [SerializeField] private Transform balloonContainer;

        // ── Pool ─────────────────────────────────────────────────────────────────
        private readonly List<GameObject> _pool = new List<GameObject>();
        private int _activeCount = 0;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            GameEvents.OnGameReset   += HandleReset;
        }

        private void OnDisable()
        {
            GameEvents.OnGameReset   -= HandleReset;
        }

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        // ── Spawn Loop ───────────────────────────────────────────────────────────

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(config.spawnInterval);

                if (GameStateController.Instance != null && !GameStateController.Instance.IsPlaying) continue;
                if (_activeCount >= config.maxBalloonsOnScreen) continue;

                SpawnBalloon();
            }
        }

        private void SpawnBalloon()
        {
            GameObject balloon = GetFromPool();
            float x = Random.Range(-config.spawnXRange, config.spawnXRange);
            balloon.transform.position = new Vector3(x, config.spawnYOffset, -1f);

            float scale = Random.Range(config.minScale, config.maxScale);
            balloon.transform.localScale = Vector3.one * scale;

            Color color = config.palette[Random.Range(0, config.palette.Length)];

            var controller = balloon.GetComponent<BalloonController>();
            controller.Initialize(config, color, OnBalloonDeactivated);

            balloon.SetActive(true);
            _activeCount++;
        }

        // ── Pool Helpers ─────────────────────────────────────────────────────────

        private GameObject GetFromPool()
        {
            foreach (var obj in _pool)
            {
                if (!obj.activeInHierarchy) return obj;
            }

            var newBalloon = Instantiate(balloonPrefab, balloonContainer);
            newBalloon.SetActive(false);
            _pool.Add(newBalloon);
            return newBalloon;
        }

        private void OnBalloonDeactivated()
        {
            _activeCount = Mathf.Max(0, _activeCount - 1);
        }

        // ── Reset ────────────────────────────────────────────────────────────────

        private void HandleReset()
        {
            foreach (var obj in _pool)
                obj.SetActive(false);
            _activeCount = 0;
        }
    }
}
