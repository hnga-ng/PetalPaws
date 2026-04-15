using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuHandler : MonoBehaviour
{
    public void StartGame()
    {
        // This loads the scene named "MainGame" from your list
        SceneManager.LoadScene("MainGame");
    }

    // Make sure this method is public and has no parameters so it appears in the Button OnClick() list
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stop play mode when testing inside the Editor
        EditorApplication.isPlaying = false;
#else
        // Quit the built application
        Application.Quit();
#endif
    }
}