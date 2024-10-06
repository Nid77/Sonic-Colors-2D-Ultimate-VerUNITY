
using System;
using UnityEngine;


public class EnnemyAction : MonoBehaviour
{


    //Deplacement 
    public bool start_left;

    public float speed;
    private int coef=1;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero; 
    public SpriteRenderer spriteRenderer;

    //Verification 
    public Transform groundCheckL; 
    public Transform groundCheckR;
    private bool isGroundedRight;
    private bool isGroundedLeft;
    public LayerMask groundLayer;
   
    public bool targeted=false;
    //Animation
     private bool fliped=false;
    public Animator animator;

    public GameObject target;

    void Start()
    {
        if(start_left){
            coef=-1;
        }

    }

    void Update()
    {
        // il doit tourner quand il rencontre un mur
         isGroundedRight = Physics2D.Raycast(groundCheckL.position, Vector2.down, 0.1f, groundLayer);
         isGroundedLeft = Physics2D.Raycast(groundCheckR.position, Vector2.down, 0.1f, groundLayer);
        


        if(isGroundedLeft && isGroundedRight ){
                fliped=false;
        }
        

        if((!fliped && isGroundedLeft && !isGroundedRight) || (!fliped && !isGroundedLeft && isGroundedRight) ){
            coef*=-1;
           
            fliped=true;
            Debug.Log("oui");
        }

        if(SonicMovement.instance.isJumping==true && SonicMovement.instance.targeted==gameObject){  
            target.SetActive(true); 

        }else{
           target.SetActive(false); 
        }
        


        Flip(speed*coef);
        Move(speed*coef*Time.deltaTime);
        
        animator.SetFloat("speed",Math.Abs(rb.velocity.x));
        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && SonicMovement.instance.isJumping==true)
        {
            Destroy(gameObject);
            SonicMovement.instance.Jump(40);
            SonicBoost.instance.takeBoost(12);
   
        }
        else if(collision.gameObject.CompareTag("Player")){
            SonicState.instance.Damage();
        }
    }


    void Move(float speed){
        Vector3 targetVelocity = new Vector2(speed,rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity,targetVelocity,ref velocity, 0.05f);
    }

    void Flip(float horizon){
        if(horizon>0){
            spriteRenderer.flipX=true;
        }else{
            spriteRenderer.flipX=false;
        }
    }


    
}
