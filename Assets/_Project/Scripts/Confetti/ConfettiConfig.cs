using UnityEngine;

namespace ConfettiFlow.Confetti
{
    /// <summary>
    /// ScriptableObject holding all tuning data for confetti burst behavior and physics.
    /// Create via Assets > Create > ConfettiFlow > ConfettiConfig.
    /// </summary>
    [CreateAssetMenu(fileName = "ConfettiConfig", menuName = "ConfettiFlow/ConfettiConfig")]
    public class ConfettiConfig : ScriptableObject
    {
        [Header("Burst")]
        [Tooltip("Number of confetti pieces spawned per balloon pop.")]
        [Range(5, 60)]
        public int burstCount = 20;

        [Tooltip("Min/Max speed of each piece on burst (units/sec).")]
        public float minBurstSpeed = 2f;
        public float maxBurstSpeed = 5f;

        [Header("Gravity & Drift")]
        [Tooltip("Downward acceleration applied to each piece (simulated gravity).")]
        public float gravity = 2.5f;

        [Tooltip("How much tilt input affects horizontal velocity (scale factor).")]
        public float tiltInfluence = 1.8f;

        [Tooltip("Air drag — reduces velocity each frame (0=no drag, 1=instant stop).")]
        [Range(0f, 0.1f)]
        public float drag = 0.012f;

        [Header("Lifetime")]
        [Tooltip("Minimum time a confetti piece stays on screen (seconds).")]
        public float minLifetime = 6f;

        [Tooltip("Maximum time a confetti piece stays on screen (seconds).")]
        public float maxLifetime = 12f;

        [Header("Size")]
        public float minSize = 0.06f;
        public float maxSize = 0.16f;

        [Header("Rotation")]
        [Tooltip("Max random rotation speed (degrees/sec).")]
        public float maxRotationSpeed = 180f;

        [Header("Touch Swipe")]
        [Tooltip("Radius around a touch point within which confetti gets pushed.")]
        public float swipeInfluenceRadius = 0.8f;

        [Tooltip("Force applied to confetti within the swipe radius.")]
        public float swipeForceMagnitude = 4f;

        [Header("Pool")]
        [Tooltip("Total confetti pieces pre-allocated in the pool.")]
        [Range(50, 500)]
        public int poolSize = 200;
    }
}
