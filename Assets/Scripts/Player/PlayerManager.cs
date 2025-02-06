using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Component References")]
    public PlayerMovement Movement { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public Animator PlayerAnimator { get; private set; }
    
    [Header("Visual References")]
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private GameObject playerHourglass;
    [SerializeField] private GameObject heldBone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        Movement = GetComponent<PlayerMovement>();
        Combat = GetComponent<PlayerCombat>();
        PlayerAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Level1")
        {
            playerHourglass.SetActive(true);
        }
        transform.position = GameObject.Find("InitialSpawn").transform.position;
    }
}
