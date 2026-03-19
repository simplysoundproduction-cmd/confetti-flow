using UnityEngine;

namespace ConfettiFlow.Gameplay
{
    /// <summary>
    /// ScriptableObject holding all tuning data for balloon spawning behavior.
    /// Create via Assets > Create > ConfettiFlow > BalloonSpawnConfig.
    /// </summary>
    [CreateAssetMenu(fileName = "BalloonSpawnConfig", menuName = "ConfettiFlow/BalloonSpawnConfig")]
    public class BalloonSpawnConfig : ScriptableObject
    {
        [Header("Spawn Rate")]
        [Tooltip("How many seconds between each balloon spawn.")]
        [Range(0.3f, 5f)]
        public float spawnInterval = 1.2f;

        [Tooltip("Max balloons alive on screen at once.")]
        [Range(1, 20)]
        public int maxBalloonsOnScreen = 8;

        [Header("Rise Speed")]
        [Tooltip("Minimum rise speed (units/sec).")]
        public float minRiseSpeed = 0.8f;

        [Tooltip("Maximum rise speed (units/sec).")]
        public float maxRiseSpeed = 1.8f;

        [Header("Horizontal Drift")]
        [Tooltip("Max horizontal sway amplitude per balloon.")]
        public float maxDriftAmplitude = 0.3f;

        [Tooltip("Sway frequency.")]
        public float driftFrequency = 0.8f;

        [Header("Spawn Zone")]
        [Tooltip("Horizontal range to spawn balloons within (centered on x=0).")]
        public float spawnXRange = 2.5f;

        [Tooltip("Y position below screen where balloons spawn.")]
        public float spawnYOffset = -6f;

        [Tooltip("Y position above screen where balloons are despawned if not popped.")]
        public float despawnYOffset = 6f;

        [Header("Scale")]
        [Tooltip("Min balloon scale.")]
        public float minScale = 0.7f;

        [Tooltip("Max balloon scale.")]
        public float maxScale = 1.2f;

        [Header("Palette")]
        [Tooltip("Colors to randomly assign to balloons. Defines confetti burst color too.")]
        public Color[] palette = new Color[]
        {
            new Color(1.00f, 0.60f, 0.70f), // soft pink
            new Color(0.70f, 0.85f, 1.00f), // sky blue
            new Color(0.75f, 1.00f, 0.75f), // mint green
            new Color(1.00f, 0.95f, 0.60f), // butter yellow
            new Color(0.85f, 0.70f, 1.00f), // lavender
            new Color(1.00f, 0.80f, 0.55f), // peach
        };
    }
}
