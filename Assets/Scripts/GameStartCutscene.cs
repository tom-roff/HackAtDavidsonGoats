using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartCutscene : MonoBehaviour
{
    public GameObject player;          // Reference to the player object
    public GameObject tourGroup;       // Reference to the tour group object
    public GameObject gate;            // Reference to the gate object
    public GameObject dialogueText; // Reference to the on-screen dialogue UI

    public float tourGroupSpeed = 2f;  // Speed of the tour group
    public float playerSpeed = 2f;     // Speed of the player's automatic movement
    public Transform tourDestination;  // Target destination for the tour group
    public Transform playerDestination; // Target destination for the player before the gate closes

    private bool isMovingTourGroup = false;
    private bool isMovingPlayer = false;
    
    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // Move the tour group away
        isMovingTourGroup = true;
        while (Vector3.Distance(tourGroup.transform.position, tourDestination.position) > 0.1f)
        {
            tourGroup.transform.position = Vector3.MoveTowards(tourGroup.transform.position, tourDestination.position, tourGroupSpeed * Time.deltaTime);
            yield return null;
        }
        isMovingTourGroup = false;

        // Display internal dialogue
        ShowDialogue();

        // Move the player automatically
        isMovingPlayer = true;
        while (Vector3.Distance(player.transform.position, playerDestination.position) > 0.1f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerDestination.position, playerSpeed * Time.deltaTime);
            yield return null;
        }
        isMovingPlayer = false;

        // Close the gate behind the player
        CloseGate();
    }

    void ShowDialogue()
    {
        dialogueText.SetActive(true);
        Invoke("HideDialogue", 5f);
    }

    void HideDialogue()
    {
        dialogueText.SetActive(false);
    }

    void CloseGate()
    {
        if (gate != null)
        {
            // Example: Animate the gate closing
            gate.SetActive(true); // Hide the gate, replace with animation if needed
        }
    }
}
