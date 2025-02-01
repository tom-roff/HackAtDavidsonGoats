using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Health")]
    [SerializeField] private TMP_Text healthText;

    // [Header("Pause")]
    // [SerializeField] private GameObject pauseMenu;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealthUI(int currentHealth)
    {
        healthText.text = $"Health: {currentHealth}";
    }

    // public void TogglePauseMenu(bool paused)
    // {
    //     pauseMenu.SetActive(paused);
    //     Time.timeScale = paused ? 0 : 1;
    //     Cursor.visible = paused;
    //     Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    // }
}
