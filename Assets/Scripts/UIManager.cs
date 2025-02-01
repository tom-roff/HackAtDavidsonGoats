using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_FontAsset newFont;
    
    [Header("Health")]
    [SerializeField] private TMP_Text healthText;

    void Awake()
    {

        healthText.font = newFont;
        // Singleton enforcement
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Only if you want persistent UI
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}";
        }
        else
        {
            Debug.LogError("Health Text reference missing in UIManager");
        }
    }
}
