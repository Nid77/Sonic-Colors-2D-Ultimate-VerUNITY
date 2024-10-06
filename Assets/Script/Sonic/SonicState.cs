using System.Collections;
using UnityEngine;

public class SonicState : MonoBehaviour
{
    public static SonicState instance;
    public bool IsInvincible;
    public float InvincibilityFlashDelay;
    public float InvincibilityDelay;
    public SpriteRenderer sprite;

    public Transform playerspawn;
     public Transform sonic;
     private Animator fadeSystem ;

     private const float deathAnimaion=1.2f;


    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance dans la scene");
        }

        instance = this;

        fadeSystem = GameObject.FindGameObjectWithTag("FadeSystem").GetComponent<Animator>();
    
    }


    public void Damage(){
        if(!IsInvincible){
            if(UI.instance.ringsCount==0&& UI.instance.lifesCount>0 ){
            Death();
            UI.instance.RemoveLife();
            StartCoroutine(ReplacePlayer());
        }else if(UI.instance.ringsCount==0 && UI.instance.lifesCount==0 ){
            Death();
            GameOverManager.instance.OnPlayerDeath();
        }else{           
            UI.instance.RemoveRings();     
            IsInvincible=true;
             SonicMovement.instance.Jump(30);
            SonicMovement.instance.animator.SetTrigger("damage");
            StartCoroutine(InvincibleFlash());
            StartCoroutine(InvincibleDelay());
            
        }
        }
        
    }
    
    public void Death(){

        SonicMovement.instance.enabled=false;
        SonicMovement.instance.rb.velocity=Vector3.zero;
        SonicMovement.instance.animator.SetTrigger("death");
        CameraFollow.instance.IsAlive=false;
        SonicMovement.instance.Jump(40);
        //SonicMovement.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        SonicMovement.instance.GetComponent<Collider2D>().enabled=false;
        SonicMovement.instance.GetComponent<BoxCollider2D>().enabled=false;
    }

    public void Respawn(){ // fonction utile lorsque sonic n'est pas present par default dans une scene
        SonicMovement.instance.enabled=true;
        SonicMovement.instance.animator.SetTrigger("Respawn");
        CameraFollow.instance.IsAlive=true;
        SonicMovement.instance.GetComponent<Collider2D>().enabled=true;
        SonicMovement.instance.GetComponent<BoxCollider2D>().enabled=true;
    }

    public IEnumerator InvincibleFlash(){
        while(IsInvincible){
            sprite.color= new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(InvincibilityFlashDelay);
            sprite.color= new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(InvincibilityFlashDelay);
            SonicMovement.instance.enabled=true;
            //SonicMovement.instance.rb.velocity=Vector3.zero;
            SonicMovement.instance.TakeDamage(gameObject);

        }
        
    }

    public IEnumerator InvincibleDelay(){
         yield return new WaitForSeconds(InvincibilityDelay);
         IsInvincible=false;
         SonicMovement.instance.enabled=false;
    }

     

    public IEnumerator ReplacePlayer(){ // METTRE UNE TRANSITION TITLECARD a la place d'un foundu
        //fadeSystem.SetTrigger("FadeIn");
        yield return new WaitForSeconds(deathAnimaion);
        Respawn();
        sonic.transform.position = playerspawn.position;
    }
}
