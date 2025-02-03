using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().name == "TutorialLevel")
            {
                SceneManager.LoadScene(sceneName:"Casino");
            }

            else if(SceneManager.GetActiveScene().name == "Level1")
            {
               SceneManager.LoadScene(sceneName:"MainMenu"); 
            }
            
        }
    }
}
