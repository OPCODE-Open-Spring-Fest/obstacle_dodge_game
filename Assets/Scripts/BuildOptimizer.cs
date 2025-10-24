using UnityEngine;

[System.Serializable]
public class BuildOptimizer : MonoBehaviour
{
    [Header("Build Optimization Settings")]
    [SerializeField] private bool enableOptimizations = true;
    [SerializeField] private bool stripUnusedCode = true;
    
    [Header("Texture Optimization")]
    [SerializeField] private TextureImporterFormat textureFormat = TextureImporterFormat.DXT1;
    [SerializeField] private int maxTextureSize = 1024;
    [SerializeField] private bool enableMipMaps = true;
    
    void Start()
    {
        if (enableOptimizations)
        {
            ApplyBuildOptimizations();
        }
    }
    
    void ApplyBuildOptimizations()
    {
        // quality setting for better performance
        QualitySettings.SetQualityLevel(2, true); // set to "Good" quality
        
        // rendering settings
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowDistance = 50f;
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        
        // texture settings
        QualitySettings.masterTextureLimit = 0;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        
        // particle settings
        QualitySettings.particleRaycastBudget = 256;
        
        // lod settings
        QualitySettings.lodBias = 0.5f;
        QualitySettings.maximumLODLevel = 0;
        
        // rendering pipeline
        
        Debug.Log("Build optimizations applied successfully!");
    }
    
    [System.Serializable]
    public enum TextureImporterFormat
    {
        DXT1,
        DXT5,
        ETC2_RGB4,
        ETC2_RGBA8,
        ASTC_4x4,
        ASTC_6x6,
        ASTC_8x8
    }
}
