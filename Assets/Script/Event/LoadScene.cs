using UnityEngine.SceneManagement;
using UnityEngine;


public class LoadResult : MonoBehaviour
{
    public string scene;
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            SceneManager.LoadScene(scene);
        }
    }
   

}
