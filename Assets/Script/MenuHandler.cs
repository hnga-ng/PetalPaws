using UnityEngine;
using UnityEngine.SceneManagement; // This is the important line!

public class MenuHandler : MonoBehaviour
{
    public void StartGame()
    {
        // This loads the scene named "MainGame" from your list
        SceneManager.LoadScene("MainGame");
    }
}