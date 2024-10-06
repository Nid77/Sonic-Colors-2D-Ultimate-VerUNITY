using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;

    public GameObject settings;
    public void StartNewGame(){
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Continue(){
        SceneManager.LoadScene("WorldMap");
    }

    public void OpenWindowSettings(){
        settings.SetActive(true);
    }

    public void ClosedWindowSettings(){
        settings.SetActive(false);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
