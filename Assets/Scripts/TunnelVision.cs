using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TunnelVision : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float minIntensity = 0f;
    
    private Vignette vignette;
    private GameManager gameManager;

    void Start()
    {
        // Get references
        gameManager = GameManager.Instance;
        
        // Get vignette effect
        if (postProcessVolume.profile.TryGet(out Vignette vig))
        {
            vignette = vig;
        }
        else
        {
            Debug.LogError("No Vignette effect found in Post Process Volume!");
        }
    }

    void Update()
    {
        UpdateVignette();
    }

    void UpdateVignette()
    {
        if (vignette == null || gameManager == null) return;

        // Get health values from GameManager
        float currentHealth = gameManager.GetPlayerCurrentHealth();
        float maxHealth = gameManager.GetPlayerMaxHealth();
        
        // Prevent division by zero
        if (maxHealth <= 0) return;

        // Calculate intensity based on health percentage
        float healthPercentage = currentHealth / maxHealth;
        float intensity = Mathf.Lerp(maxIntensity, minIntensity, healthPercentage);

        // Apply intensity to vignette
        vignette.intensity.Override(intensity);
    }
}
