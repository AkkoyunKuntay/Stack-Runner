using UnityEngine;

[CreateAssetMenu(menuName = "StackRunner/Platform Generator Settings")]
public class PlatformGenerationSettings : ScriptableObject
{
    [Header("Prefab & Pool")]
    public PlatformView platformPrefab;
    [Range(5, 100)] public int initialPoolSize = 20;

    [Header("Geometry")]
    public float platformWidth = 1.0f;   
    public float platformDepth = 1.0f;   
    public float lateralOffset = 2f;   

    [Header("Perfect Cut")]
    [Tooltip("Tolerance for perfect cut")]
    public float perfectTolerance = 0.05f;
}
