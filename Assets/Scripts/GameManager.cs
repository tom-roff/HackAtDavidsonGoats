using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private int playerCurrentHealth;

    [Header("Player References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform initialSpawnPoint;

    [Header("Game State")]
    [SerializeField] private float respawnDelay = 1f;
    private Vector3 currentCheckpoint;
    private bool isGamePaused;


    [SerializeField] private float soulDrainRateInSeconds = 1;
    //Amount of time before souls start draining
    [SerializeField] private float timeSafeAtStart = 1;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeGame()
    {
        playerCurrentHealth = playerMaxHealth;

        if (initialSpawnPoint != null)
        {
            currentCheckpoint = initialSpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No initial spawn point set in GameManager!");
            currentCheckpoint = Vector3.zero;
        }
    }

    public void UpdateCheckpoint(Vector3 newCheckpointPosition)
    {
        currentCheckpoint = newCheckpointPosition;
        if (showDebugInfo)
        {
            Debug.Log($"Checkpoint updated to: {newCheckpointPosition}");
        }
    }

    public void DamagePlayer(int damage)
    {
        playerCurrentHealth -= damage;

        UIManager.Instance.UpdateHealthUI(playerCurrentHealth);
        
        // Optional: Add hit effects/animations here
        
        if (playerCurrentHealth <= 0)
        {
            KillPlayer();
        }
        
        player.playerHourglass.GetComponent<HourglassManager>().UpdateSoulMat(playerCurrentHealth , playerMaxHealth);
    }

    public void HealPlayer(int amount)
    {
        playerCurrentHealth = Mathf.Min(playerCurrentHealth + amount, playerMaxHealth);
        UIManager.Instance.UpdateHealthUI(playerCurrentHealth);
    }

    public void KillPlayer()
    {
        if (player != null)
        {
            // Disable player controls
            player.enabled = false;
            
            // Optional: Play death animation/effects here
            
            // Respawn after delay
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
    }

    private void RespawnPlayer()
    {
        playerCurrentHealth = playerMaxHealth;

        if (player != null)
        {
            // Reset player position
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = currentCheckpoint;
                controller.enabled = true;
            }

            // Re-enable player controls
            player.enabled = true;
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        // Add pause menu activation here
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        // Add pause menu deactivation here
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Optional: Add these features as needed
    private int souls;
    private float timeElapsed;
    private int deathCount;

    public void AddSouls(int amount)
    {
        souls += amount;
        // Add UI update here
    }

    public bool SpendSouls(int amount)
    {
        if (souls >= amount)
        {
            souls -= amount;
            // Add UI update here
            return true;
        }
        return false;
    }

    void Update()
    {
        // Example pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (!isGamePaused)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    void Start(){
        InvokeRepeating("DrainPlayerHealth", timeSafeAtStart, soulDrainRateInSeconds);
    }

    private void DrainPlayerHealth(){
        // Damage Player amount of Souls to Drain
        DamagePlayer(1);
    }


    // Getter methods
    public Vector3 GetCurrentCheckpoint() => currentCheckpoint;
    public int GetSoulCount() => souls;
    public float GetTimeElapsed() => timeElapsed;
    public int GetDeathCount() => deathCount;
    public bool IsGamePaused() => isGamePaused;
    public int GetPlayerCurrentHealth() => playerCurrentHealth;
    public int GetPlayerMaxHealth() => playerMaxHealth;
}
