using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBoost : MonoBehaviour
{
    public float maxboost=100;
    public float currentboost;

    public boost_bar boost;
    
    public static SonicBoost instance;


    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance dans la scene");
        }

        instance = this;
    }
    
    void Start()
    {
        boost.SetInitBoost();
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)){
            takeBoost(12);
        }

    

        
    }

    public void takeBoost(float nb){
        boost.AddBoost(nb);
        currentboost=boost.getBoost();
    }

    public void loseBoost(){
        boost.RemoveBoost(1f);;
        currentboost=boost.getBoost();
    }

    public bool ZeroBoost(){
        if(currentboost==0){
            return true;
        }
        return false;
    }
}
