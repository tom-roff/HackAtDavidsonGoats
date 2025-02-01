using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool reached = false;
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player")) && (reached == false))
        {
            GameManager gameManager = GameManager.Instance;
            
            if (gameManager != null)
            {
                // Update the checkpoint position to this checkpoint's position
                gameManager.UpdateCheckpoint(transform.position);
                reached = true;
                Debug.Log("Checkpoint reached at: " + transform.position);
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}
