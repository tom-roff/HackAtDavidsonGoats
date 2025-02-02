using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject bettingCanvas;
    private int index;
    private bool isDialogueActive = false;
    private bool canSkipText = false; // Prevents skipping text until dialogue starts
    private bool dialogueFinished = false;
    

    void Start()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false); // Hide the dialogue panel at the start
        bettingCanvas.SetActive(false);
    }

    public void StartDialogue()
    {
        if (!isDialogueActive && !dialogueFinished) // Prevents restarting if dialogue finished
        {
            isDialogueActive = true;
            canSkipText = false; // Prevent skipping text until it starts
            gameObject.SetActive(true); // Show dialogue box
            index = 0;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

    public void EndDialogue(){
        isDialogueActive = false;
        gameObject.SetActive(false);
        bettingCanvas.SetActive(true);
        Debug.Log("Betting Canvas activated: " + bettingCanvas.activeSelf);
        dialogueFinished = true;
    }

    void Update()
    {
        if (isDialogueActive && canSkipText && Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        canSkipText = true; // Enable skipping once full text is displayed
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            canSkipText = false; // Disable skipping while new text is typing
            StartCoroutine(TypeLine());
        }
        else
        {
            textComponent.text = string.Empty;
            isDialogueActive = false;
            canSkipText = false; // Prevent skipping after dialogue ends
            EndDialogue(); // Hide dialogue panel when done
        }
    }
}
