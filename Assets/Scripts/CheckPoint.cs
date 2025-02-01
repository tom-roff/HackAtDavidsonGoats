using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.Instance;
            
            if (gameManager != null)
            {
                // Update the checkpoint position to this checkpoint's position
                gameManager.UpdateCheckpoint(transform.position);
                Debug.Log("Checkpoint reached at: " + transform.position);
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}
