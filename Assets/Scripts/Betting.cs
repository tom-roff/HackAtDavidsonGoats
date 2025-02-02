using UnityEngine;

public class Betting : MonoBehaviour
{
    public GameObject bettingCanvas; // Reference to the betting UI


    public void ChooseBet(int betID)
    {
        // Apply effects based on bet choice (you can expand this logic)
        switch (betID)
        {
            case 1:
                Debug.Log("Player chose Bet 1: Higher Damage, Less Souls");
                break;
            case 2:
                Debug.Log("Player chose Bet 2: Lower Damage, More Souls");
                break;
            case 3:
                Debug.Log("Player chose Bet 3: Enemies Become Harder, More Rewards");
                break;
        }

        // Close betting UI and move to gameplay
        bettingCanvas.SetActive(false);
    }
}
