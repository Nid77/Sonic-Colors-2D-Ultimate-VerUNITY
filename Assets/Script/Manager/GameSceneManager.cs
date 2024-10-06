using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public bool isPlayerPresentbyDefault =false;
    public static GameSceneManager instance;

    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de SceneManager dans la scene");
        }

        instance = this;
    }
}
