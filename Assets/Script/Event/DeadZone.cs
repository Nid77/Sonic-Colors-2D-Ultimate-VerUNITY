using System.Collections;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private Transform playerspawn;
    private Animator fadeSystem ;
    
    public void Awake(){
        playerspawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        fadeSystem = GameObject.FindGameObjectWithTag("FadeSystem").GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            
            SonicState.instance.Death();
            if(UI.instance.lifesCount==0){
                GameOverManager.instance.OnPlayerDeath();
            }else{
                UI.instance.RemoveLife();
                StartCoroutine(SonicState.instance.ReplacePlayer());
            }
           
            

        }
    }

    
}
