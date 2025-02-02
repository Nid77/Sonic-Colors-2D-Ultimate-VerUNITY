using System;
using UnityEngine;

public class SonicMovement : MonoBehaviour
{
    //Objet Reference
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Controller controller;
    public GameObject targeted;
    public static SonicMovement instance;
    private LineRenderer lineRenderer;
    public Animator animator;

    //Physic Value
    public float currentSpeed; 
    public float speedIncreaseRate;     // Taux d'augmentation de la vitesse par seconde
    public float TrueSpeed;             // valeur en prenant en compte le decalage (currentSpeed + basespeed )
    public float maxSpeed;              // Vitesse max
    public float baseSpeed;             // vitesse de base de sonic ( pas 0 sinon ya pas d'inertie quand il demare et c'est long )
    public float cruiseSpeed = 15f;     // Vitesse de croisière
    public float boostSpeed = 40f;      // Vitesse avec Boost
    public float acceleration = 2f;     // Accélération normale
    public float deceleration = 3f;     // Décélération quand on arrête
    private SpeedPhase currentPhase = SpeedPhase.Slow;

    public float Inertia = 1.5f ;       // l'inertie quand il change de cote ou arete de courir 
    public float airResistance;
    public float lastSpeedDirection;    
    public float knockbackDistance=80f;   
    public float jumpForce; 
    
    
    //Move Input
    public float horizonMov;
    private float vertiMov;

     
    //Action value 
    public bool homingAttackPossible=false; 
    public bool isHomingAttack=false;   // Faire le homing attack dans le FixedUpdate
    public bool MoveJump=false;         // Faire le jump dans le FixedUpdate
    public float homingSpeed; 
    public bool isfalling; 
    public bool isJumping=false;
    public bool isGrounded; 
    private bool isTakingDamage = false;
    public bool fliped = false;
    public bool InertiaPossible=false;
    public bool isLower=false; 
    public bool SpinDash=false;
    private float offsetFlipChange = 30f;// A partir de quel val il peut prendre l'inertie


    //Collision
    private LayerMask layersToIgnoreHomingAttack;
    public LayerMask collisionGroundLayer;
    public Transform groundCheck; 
    public float groundCheckRadius;
    public float rayThickness = 0.1f; 
    public float xOffsetWallCollision = 0.1f;
    private CapsuleCollider2D capsuleCollider;
    public LayerMask collisionWallMask;
    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;


    public enum SpeedPhase
    {
        Slow,          // Lent
        Accelerating,  // Accélération
        Cruising,      // Vitesse de croisière
        MaxSpeed,      // Vitesse maximale
        Boosting       // Boost activé
    }




    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de SonicMovement dans la scene");
        }

        instance = this;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    
    }


    void Start(){
        layersToIgnoreHomingAttack = LayerMask.GetMask("Enemy", "Player", "UI");
        collisionGroundLayer = 1 << LayerMask.NameToLayer("Background");
        collisionWallMask = 1 << LayerMask.NameToLayer("Background");
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        currentSpeed = baseSpeed;
    }


    void Update()
    {   
        //Detection de collisions
        DetectCollisions();

        //Mouvement de sonic
        if(controller.keyDictionary["Right"]){
            horizonMov=1;
        }else if(controller.keyDictionary["Left"]){
            horizonMov=-1;
        }else{
            horizonMov=0;
        }


        //Faire un saut
        if (controller.keyDictionary["Jump"] && isGrounded){   
            isJumping=true;
            MoveJump=true;
        } // Faire un homingAttackPossible
        else if(controller.keyDictionary["Jump"] && isJumping){
            homingAttackPossible=true;
        }

        // Si il tombe
        if (!(rb.velocity.y > 0f) && !isGrounded){
            isfalling = true;
        }else{
            isfalling = false;
        }

        //Pour enlever la phase de saut
        if (isGrounded && !(rb.velocity.y > 0f) && !MoveJump){
            isJumping=false;    
        }

        
        if(controller.keyDictionary["Down"]){

            // le faire baissé 
            if (currentSpeed==baseSpeed && !isLower){ 
                animator.SetTrigger("lower");
                isLower=true;
            }
            else // Faire le Spindash
            { 
                SpinDash=true;
            }

        }else{
            if(isLower){
                animator.SetTrigger("back");
                isLower=false;
            }
            SpinDash=false;
        }


        //L'enemmie le plus proche sera attaqué
        if(SonicActions.instance.objs.Count > 0 && !isHomingAttack)
        {
            targeted=FindNearEnenmy();
            if (targeted != null && isJumping && homingAttackPossible && Input.GetKeyDown(KeyCode.Space) && !isfalling){
                isHomingAttack = true;
            }
        }


        // Animation
        animator.SetBool("isGrounded",isGrounded);
        animator.SetBool("isjumping",isJumping);
        animator.SetFloat("speed",currentSpeed);
        animator.SetBool("isfalling",isfalling);
        animator.SetBool("SpinDash",SpinDash);
        animator.SetBool("baseSpeed", currentSpeed > baseSpeed);

        
    }

    void FixedUpdate(){
        // Code de la physique ici

        if(isHomingAttack){
            HomingAttack();
        }
        if(MoveJump){
             Jump(jumpForce);
             MoveJump=false;
        }

        Flip(horizonMov);
        Move(horizonMov);
        UpdateSpeedPhase();
    }

    void DetectCollisions()
    {
        //Detecter si sonic est sur le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,collisionGroundLayer );


        // Detecter su sonic touche un mur sur le cote droit et gauche
        float rayLength = capsuleCollider.size.y;
        Vector2 leftRayOrigin = new Vector2(capsuleCollider.bounds.min.x - xOffsetWallCollision, capsuleCollider.bounds.center.y);
        Vector2 rightRayOrigin = new Vector2(capsuleCollider.bounds.max.x + xOffsetWallCollision, capsuleCollider.bounds.center.y);


        hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.up, rayLength, collisionWallMask);
        if (hitLeft.collider != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            currentSpeed = baseSpeed;
            Debug.Log("Collide left");
        }

         hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.up, rayLength, collisionWallMask);
        if (hitRight.collider != null)
        {
           rb.velocity = new Vector2(0, rb.velocity.y);
           currentSpeed = baseSpeed;
           Debug.Log("Collide right");
        }

        //Debug.DrawRay(leftRayOrigin, Vector2.up * rayLength, Color.red);
        //Debug.DrawRay(rightRayOrigin, Vector2.up * rayLength, Color.red);

    }

    public void Move(float _horizon){
        

        //Application de l'inertie en fonction de certaines conditions
        //LA condition c'est si il n'appuie sur les boutons / si il fait un spindahs ou il change de cote
        if ( (!controller.keyDictionary["Right"] && !controller.keyDictionary["Left"]) || InertiaPossible || SpinDash && isGrounded ){

            if(currentSpeed>baseSpeed){ 
                currentSpeed -= ((currentSpeed * Inertia) * Time.deltaTime);
                currentSpeed = Mathf.Max(currentSpeed, baseSpeed);
            }

            if(currentSpeed==baseSpeed && InertiaPossible){
                InertiaPossible=false;
                animator.SetTrigger("unflip");
            }
            
        }else{ // Mouvement de base 

            lastSpeedDirection = horizonMov < 0 ? -1 : 1;

            if (currentSpeed < maxSpeed){
                currentSpeed += (speedIncreaseRate * Time.deltaTime);
                currentSpeed = Mathf.Min(currentSpeed,maxSpeed);
            }
           
        }


        TrueSpeed = lastSpeedDirection * (currentSpeed - baseSpeed);
        rb.velocity = new Vector2(TrueSpeed, rb.velocity.y);
    }

    public void Jump(float jumpForce){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void HomingAttack()
    {
        if (targeted!=null){
            transform.position = Vector2.MoveTowards(transform.position, targeted.transform.position, homingSpeed * Time.deltaTime);
        }else{
            isHomingAttack = false;
        }
        
        homingAttackPossible=false;
    }
    
    public void TakeDamage(GameObject collision){
        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        transform.Translate(knockbackDirection * -knockbackDistance);
        Debug.Log("RECUL");
    }
    

    void Flip(float _horizon){
        if (_horizon>=0f){
            fliped = spriteRenderer.flipX;
            spriteRenderer.flipX=false;
        }
        else if(_horizon<0f){
            fliped = !spriteRenderer.flipX;
            spriteRenderer.flipX = true;
        }

        if (fliped && currentSpeed>baseSpeed+offsetFlipChange){
            InertiaPossible=true;
            animator.SetTrigger("flip");
        }  
    }

    
    public GameObject FindNearEnenmy(){
        
        GameObject nearObj = SonicActions.instance.objs[0];

        foreach(GameObject obj in SonicActions.instance.objs) {

            if (!IsObstacleBetween(obj) && Vector3.Distance(gameObject.transform.position, obj.transform.position) < Vector3.Distance(gameObject.transform.position, nearObj.transform.position)){
                    nearObj = obj;
            }
            
        }

       return nearObj; 
    }

    public bool IsObstacleBetween(GameObject obstacle){
        
        RaycastHit2D hitInfo = Physics2D.Linecast(gameObject.transform.position, obstacle.transform.position,~layersToIgnoreHomingAttack);
        return hitInfo.collider != null;
         
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void UpdateSpeedPhase()
    {
        if (currentSpeed < baseSpeed)
        {
            currentPhase = SpeedPhase.Slow;
        }
        else if (currentSpeed < cruiseSpeed)
        {
            currentPhase = SpeedPhase.Accelerating;
        }
        else if (currentSpeed < maxSpeed)
        {
            currentPhase = SpeedPhase.Cruising;
        }
        else if (currentSpeed < boostSpeed)
        {
            currentPhase = SpeedPhase.MaxSpeed;
        }
        else
        {
            currentPhase = SpeedPhase.Boosting;
        }

    }

}
