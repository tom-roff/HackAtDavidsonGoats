using UnityEngine;

public class Killbox : MonoBehaviour
{
    public int playerDamage = 20;

    protected GameManager gameManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Always apply damage first
                if(gameManager.environmentDealHalf)
                {
                    gameManager.DamagePlayer(playerDamage / 2);
                }
                else
                {
                    gameManager.DamagePlayer(playerDamage);
                }

                gameManager.KillPlayer();
            
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}
