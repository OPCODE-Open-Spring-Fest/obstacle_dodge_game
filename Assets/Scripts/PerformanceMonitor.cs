using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("Performance Monitoring")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private bool showMemoryUsage = true;
    [SerializeField] private bool showDrawCalls = true;
    [SerializeField] private bool logPerformanceData = false;
    
    [Header("UI References")]
    [SerializeField] private Text fpsText;
    [SerializeField] private Text memoryText;
    [SerializeField] private Text drawCallsText;
    
    [Header("Performance Targets")]
    [SerializeField] private float targetFPS = 60f;
    [SerializeField] private float warningFPS = 45f;
    [SerializeField] private float criticalFPS = 30f;
    
    private float fps;
    private float frameTime;
    private float memoryUsage;
    private int drawCalls;
    private int triangles;
    private int vertices;
    
    private List<float> fpsHistory = new List<float>();
    private int maxHistorySize = 60; // Keep 1 second of history at 60fps
    
    void Start()
    {
        // Create UI elements if not assigned
        if (fpsText == null && showFPS)
        {
            CreatePerformanceUI();
        }
        
        // Initialize performance monitoring
        InvokeRepeating(nameof(UpdatePerformanceStats), 0f, 0.1f);
    }
    
    void Update()
    {
        UpdateFPS();
        UpdateMemoryUsage();
        UpdateRenderingStats();
        
        if (logPerformanceData)
        {
            LogPerformanceData();
        }
    }
    
    void UpdateFPS()
    {
        frameTime = Time.deltaTime;
        fps = 1.0f / frameTime;
        
        // Add to history
        fpsHistory.Add(fps);
        if (fpsHistory.Count > maxHistorySize)
        {
            fpsHistory.RemoveAt(0);
        }
        
        if (fpsText != null)
        {
            string color = GetFPSColor(fps);
            fpsText.text = $"FPS: <color={color}>{fps:F1}</color>";
        }
    }
    
    void UpdateMemoryUsage()
    {
        memoryUsage = (float)System.GC.GetTotalMemory(false) / (1024 * 1024); // MB
        
        if (memoryText != null)
        {
            memoryText.text = $"Memory: {memoryUsage:F1} MB";
        }
    }
    
    void UpdateRenderingStats()
    {
        // Get rendering statistics
        drawCalls = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null ? 
            UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline.GetType().GetField("drawCalls", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(null) as int? ?? 0 : 0;
        
        if (drawCallsText != null)
        {
            drawCallsText.text = $"Draw Calls: {drawCalls}";
        }
    }
    
    string GetFPSColor(float currentFPS)
    {
        if (currentFPS >= targetFPS)
            return "green";
        else if (currentFPS >= warningFPS)
            return "yellow";
        else if (currentFPS >= criticalFPS)
            return "orange";
        else
            return "red";
    }
    
    void UpdatePerformanceStats()
    {
        // This method is called every 0.1 seconds
        if (fpsHistory.Count > 0)
        {
            float averageFPS = CalculateAverageFPS();
            float minFPS = GetMinFPS();
            float maxFPS = GetMaxFPS();
            
            if (averageFPS < targetFPS * 0.9f)
            {
                Debug.LogWarning($"Performance Warning: Average FPS ({averageFPS:F1}) below target ({targetFPS})");
            }
        }
    }
    
    float CalculateAverageFPS()
    {
        if (fpsHistory.Count == 0) return 0f;
        
        float sum = 0f;
        foreach (float f in fpsHistory)
        {
            sum += f;
        }
        return sum / fpsHistory.Count;
    }
    
    float GetMinFPS()
    {
        if (fpsHistory.Count == 0) return 0f;
        
        float min = float.MaxValue;
        foreach (float f in fpsHistory)
        {
            if (f < min) min = f;
        }
        return min;
    }
    
    float GetMaxFPS()
    {
        if (fpsHistory.Count == 0) return 0f;
        
        float max = float.MinValue;
        foreach (float f in fpsHistory)
        {
            if (f > max) max = f;
        }
        return max;
    }
    
    void LogPerformanceData()
    {
        if (Time.frameCount % 300 == 0) // Log every 5 seconds at 60fps
        {
            Debug.Log($"Performance Report - FPS: {fps:F1}, Memory: {memoryUsage:F1}MB, Draw Calls: {drawCalls}");
        }
    }
    
    void CreatePerformanceUI()
    {
        // Create a canvas for performance UI
        GameObject canvasGO = new GameObject("PerformanceCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create FPS text
        if (showFPS)
        {
            GameObject fpsGO = new GameObject("FPSText");
            fpsGO.transform.SetParent(canvasGO.transform);
            fpsText = fpsGO.AddComponent<Text>();
            fpsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            fpsText.fontSize = 24;
            fpsText.color = Color.white;
            fpsText.text = "FPS: 60.0";
            
            RectTransform fpsRect = fpsText.GetComponent<RectTransform>();
            fpsRect.anchorMin = new Vector2(0, 1);
            fpsRect.anchorMax = new Vector2(0, 1);
            fpsRect.anchoredPosition = new Vector2(10, -10);
            fpsRect.sizeDelta = new Vector2(200, 30);
        }
        
        // Create memory text
        if (showMemoryUsage)
        {
            GameObject memoryGO = new GameObject("MemoryText");
            memoryGO.transform.SetParent(canvasGO.transform);
            memoryText = memoryGO.AddComponent<Text>();
            memoryText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            memoryText.fontSize = 24;
            memoryText.color = Color.white;
            memoryText.text = "Memory: 0.0 MB";
            
            RectTransform memoryRect = memoryText.GetComponent<RectTransform>();
            memoryRect.anchorMin = new Vector2(0, 1);
            memoryRect.anchorMax = new Vector2(0, 1);
            memoryRect.anchoredPosition = new Vector2(10, -40);
            memoryRect.sizeDelta = new Vector2(200, 30);
        }
        
        // Create draw calls text
        if (showDrawCalls)
        {
            GameObject drawCallsGO = new GameObject("DrawCallsText");
            drawCallsGO.transform.SetParent(canvasGO.transform);
            drawCallsText = drawCallsGO.AddComponent<Text>();
            drawCallsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            drawCallsText.fontSize = 24;
            drawCallsText.color = Color.white;
            drawCallsText.text = "Draw Calls: 0";
            
            RectTransform drawCallsRect = drawCallsText.GetComponent<RectTransform>();
            drawCallsRect.anchorMin = new Vector2(0, 1);
            drawCallsRect.anchorMax = new Vector2(0, 1);
            drawCallsRect.anchoredPosition = new Vector2(10, -70);
            drawCallsRect.sizeDelta = new Vector2(200, 30);
        }
    }
    
    public void TogglePerformanceUI()
    {
        showFPS = !showFPS;
        showMemoryUsage = !showMemoryUsage;
        showDrawCalls = !showDrawCalls;
        
        if (fpsText != null) fpsText.gameObject.SetActive(showFPS);
        if (memoryText != null) memoryText.gameObject.SetActive(showMemoryUsage);
        if (drawCallsText != null) drawCallsText.gameObject.SetActive(showDrawCalls);
    }
}
