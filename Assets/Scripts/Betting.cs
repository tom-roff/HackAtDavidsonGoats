using UnityEngine;

public class Betting : MonoBehaviour
{
    public GameObject bettingCanvas; // Reference to the betting UI


    public void ChooseBet(int betID)
    {
        GameManager.Instance.ApplyBetEffects(betID);

        // Close betting UI and move to gameplay
        bettingCanvas.SetActive(false);
    }
}
