using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private int playerCurrentHealth;

    [SerializeField] private GameObject playerSpawnFX;

    [Header("Player References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform initialSpawnPoint;

    [Header("Game State")]
    [SerializeField] private float respawnDelay = 1f;
    private Vector3 currentCheckpoint;
    private bool isGamePaused;

    //Aetting effects
    public bool enemiesDealDouble = false;
    public bool environmentDealHalf = false;
    public bool movementSpeedDouble = false;
    public bool soulDropLess = false;


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
    }

    void Start(){
        InvokeRepeating("DrainPlayerHealth", timeSafeAtStart, soulDrainRateInSeconds);
        Transform particleSpawnTrans = player.transform;
        Vector3 particleSpawnPos = player.transform.position;
        //particleSpawnPos.y -= 10f;
        particleSpawnTrans.position = particleSpawnPos;
        GameObject spawnExplosionFX = Instantiate(playerSpawnFX, player.transform);
        spawnExplosionFX.transform.localScale *= 3;
    }

    private void DrainPlayerHealth(){
        // Damage Player amount of Souls to Drain
        if (SceneManager.GetActiveScene().name != "Casino") 
        {
            DamagePlayer(1);
        }
        
    }

    // New method to update the player's betting choices
    public void ApplyBetEffects(int betID)
    {
        switch (betID)
        {
            case 1: // Lose health/time faseter, faster movement speed
                soulDrainRateInSeconds = 2f;  // Reduce soul drain rate (less frequent)
                movementSpeedDouble = true;
                Debug.Log("Bet 1 applied: Higher Damage, Less Souls");
                break;
            case 2: // Enemies deal double damage, environment does half damage
                enemiesDealDouble = true;;
                environmentDealHalf = true;
                Debug.Log("Bet 2 applied: Lower Damage, More Souls");
                break;
            case 3: // Enemies drop less souls, lose health/time slower
                soulDropLess = true;
                soulDrainRateInSeconds = 0.5f; 
                Debug.Log("Bet 3 applied: Harder Enemies, More Rewards");
                break;
        }
    }


    // Getter methods
    public Vector3 GetCurrentCheckpoint() => currentCheckpoint;
    public bool IsGamePaused() => isGamePaused;
    public int GetPlayerCurrentHealth() => playerCurrentHealth;
    public int GetPlayerMaxHealth() => playerMaxHealth;
}
