using UnityEngine;

public class PerformanceOptimizer : MonoBehaviour
{
    [Header("Performance Settings")]
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool enableVSync = true;
    [SerializeField] private int maxLODLevel = 0;
    
    [Header("Quality Settings")]
    [SerializeField] private bool enableBatching = true;
    [SerializeField] private bool enableInstancing = true;
    
    void Start()
    {
        OptimizePerformance();
    }
    
    void OptimizePerformance()
    {
        // set target framerate
        Application.targetFrameRate = targetFrameRate;
        
        // configure vsync
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        
        // lod bias for better performance
        QualitySettings.lodBias = 0.5f;
        
        // batching for draw call
        if (enableBatching)
        {
            QualitySettings.maxQueuedFrames = 2;
        }
        
        // rendering settings
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowDistance = 50f;
        QualitySettings.shadowCascades = 2;
        
        // texture optimiztion
        QualitySettings.globalTextureMipmapLimit = 0;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        
        // particle settings
        QualitySettings.particleRaycastBudget = 256;
        
        Debug.Log("Performance optimization applied successfully!");
    }
    
    void Update()
    {
        // monitor  framerate
        if (Time.frameCount % 60 == 0) // check every 60 frames
        {
            float fps = 1.0f / Time.deltaTime;
            if (fps < targetFrameRate * 0.9f) // fps drops below 90%
            {
                Debug.LogWarning($"Performance warning: FPS dropped to {fps:F1}");
            }
        }
    }
}
