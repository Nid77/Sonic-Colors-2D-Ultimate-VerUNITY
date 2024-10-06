using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicActions : MonoBehaviour
{
    public static SonicActions instance;
    public List<GameObject> objs;

    public bool isboosted=false;

    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de SonicActions dans la scene");
        }

        instance = this;
    }
    

    void Update(){
        if(Input.GetKey(KeyCode.U) && !SonicBoost.instance.ZeroBoost()){
            isboosted=true;
            SonicBoost.instance.loseBoost();
        }else{
            isboosted=false;
        }
        SonicMovement.instance.animator.SetBool("isboosted",isboosted);
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            objs.Add(other.gameObject);
            //Debug.Log("il est dedans :"+other.gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            objs.Remove(other.gameObject);
            //Debug.Log("il a quitté :"+other.gameObject.name);
        }
    }
}