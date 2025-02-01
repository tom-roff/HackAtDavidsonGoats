using UnityEngine;

public class Killbox : MonoBehaviour
{
    [SerializeField] int playerDamage = 20;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Always apply damage first
                gameManager.DamagePlayer(playerDamage);
                
                // Handle CharacterController teleport
                CharacterController controller = other.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    other.transform.position = gameManager.GetCurrentCheckpoint();
                    controller.enabled = true;
                }
                else
                {
                    gameManager.KillPlayer();
                }
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}
