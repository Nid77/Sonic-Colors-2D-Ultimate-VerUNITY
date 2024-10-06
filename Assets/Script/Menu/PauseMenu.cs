using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    
    public static bool gameIsPaused = false;
    public GameObject PauseMenuUI;    
    public GameObject SettingsMenuUI;    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (gameIsPaused){
                Resume();
            }else{
                Paused();
            }
        }
        
    }

    void Paused(){
        SonicMovement.instance.enabled=false;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public void Resume(){
        SonicMovement.instance.enabled=true;
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void OpenWindowSettings(){
        SettingsMenuUI.SetActive(true);
    }

    public void ClosedWindowSettings(){
        SettingsMenuUI.SetActive(false);
    }


    public void LoadMainMenu(){
        DontDestroyOnLoadScene.instance.RemoveFromDontDestroyOnLoad();
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
