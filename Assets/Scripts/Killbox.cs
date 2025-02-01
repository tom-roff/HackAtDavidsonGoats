using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the GameManager instance properly
            GameManager gameManager = GameManager.Instance;
            
            if (gameManager != null)
            {
                // Handle CharacterController conflict
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
