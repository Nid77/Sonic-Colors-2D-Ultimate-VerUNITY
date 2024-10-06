using UnityEngine;
using UnityEngine.SceneManagement;
public class DontDestroyOnLoadScene : MonoBehaviour
{
    public GameObject[] objects;
    public static DontDestroyOnLoadScene instance;

    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de DontDestroyOnLoadScene dans la scene");
        }

        instance = this;
    
        foreach(var elem in objects){
            DontDestroyOnLoad(elem);
        }
    }

    public void RemoveFromDontDestroyOnLoad(){
        foreach(var elem in objects){
         SceneManager.MoveGameObjectToScene(elem,SceneManager.GetActiveScene());
        }
    }

}
