using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject GameOverUI;
    public static GameOverManager instance;

    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de GameOverManager dans la scene");
        }

        instance = this;
    }
    public void OnPlayerDeath(){
        
        // Si on decide de garder des elements d'une scene a l'autre ( peu probable )
        if(GameSceneManager.instance.isPlayerPresentbyDefault){
            DontDestroyOnLoadScene.instance.RemoveFromDontDestroyOnLoad();
        }

        GameOverUI.SetActive(true);
        
    }
  
    public void RetryButton(){
       
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameOverUI.SetActive(false);
    }

    public void MainMenuButton(){

    }

    public void QuitButton(){
        Application.Quit();
    }
}
