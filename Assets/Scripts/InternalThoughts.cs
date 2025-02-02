using UnityEngine;
using UnityEngine.SceneManagement;

public class InternalThoughts : MonoBehaviour
{
    public GameObject thoughtsScript; 
    public BasicDialogue dialogueScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && SceneManager.GetActiveScene().name == "TutorialLevel")
        {
            thoughtsScript.SetActive(true);
            dialogueScript.StartDialogue();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            thoughtsScript.SetActive(false);
        }
    }
}
