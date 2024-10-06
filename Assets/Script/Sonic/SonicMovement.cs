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
    public Animator animator; // pour envoyer la vitesse de sonic a animator
    private LayerMask layersToIgnoreHomingAttack; // Layers a ignoré ( surtout pour la detection d'ennemi )
    public LayerMask collisionGroundLayer; // Layer dans lequel on detecte le sol
    public Transform groundCheck; // variable qui sert a savoir si sonic touche le sol
    public float groundCheckRadius;


    // Physic Value
    public float currentSpeed; 
    public float baseSpeed; // vitesse de base de sonic ( pas 0 sinon ya pas d'inertie quand il demare et c'est long )
    public float speedIncreaseRate ; // Taux d'augmentation de la vitesse par seconde
    public float maxSpeed ; // Vitesse maximale du personnage
    public float TrueSpeed; // valeur en prenant en compte le decalage (currentSpeed + basespeed )
    public float Inertia ; // l'inertie quand il change de cote ou arete de courir 
    public float airResistance;
    public float lastSpeedDirection; // Sutout pour connaitre la direction de sonic
    public float knockbackDistance=80f;   
    public float jumpForce; // puissance de saut
    
    
    
    
    // Move Input
    public float horizonMov;
    private float vertiMov;

    
     
    // Boolean pour activation de mouv ou capa
    public bool homingAttackPossible=false; // Si le homingAttackPossible est possible
    public bool MoveHoming=false; // Faire le homing attack dans le FixedUpdate
    public bool MoveJump=false; // Faire le jump dans le FixedUpdate
    public float homingSpeed; 
    public bool isfalling; // si sonic tombe
    public bool isJumping=false; // si sonic saute
    public bool isGrounded; // si sonic est sur le sol
    private bool isTakingDamage = false;
    public bool fliped = false;
    public bool InirtiaPossible=false;
    public bool isLower=false; // Si sonic se baisse
    public bool SpinDash=false;//Si sonic peut faire un spindash
    private float offsetFlipChange = 30f; // A partir de quel val il peut prendre l'inertie

    // Detection de mur 
    public float rayThickness = 0.1f; // Épaisseur des rayon
    public float xOffsetWallCollision = 0.1f; // Décalage horizontal pour le point de départ des rayons
    private Collider2D collider2D; // ca boite de collision
    private BoxCollider2D boxCollider; // ca boite de collision ( qui prend en compte k'ajustement des sprite )
    public LayerMask collisionWallMask; // Detecter quel layer ?
    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;
    

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
        collider2D = GetComponent<Collider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

     


    

    void Update()
    {   
        //Detection de collisions
        
        DetectSideCollisions();

        if(controller.keyDictionary["Right"]){
            horizonMov=1;
        }else if(controller.keyDictionary["Left"]){
            horizonMov=-1;
        }else{
            horizonMov=0;
        }


        //Pouvoir saute
        if (controller.keyDictionary["Jump"] && isGrounded)
        {   
            isJumping=true;
            MoveJump=true;
        }else if(controller.keyDictionary["Jump"] && isJumping){ // Faire un homingAttackPossible
            homingAttackPossible=true;
        }
        
        // le moment ou il ne peut pas attaque
        if(isGrounded && !(rb.velocity.y > 0f) && !MoveJump){
            isJumping=false;    
        }

        // Si il tombe
        if(!(rb.velocity.y > 0f) && !isGrounded){
            isfalling=true;
        }else{
            isfalling=false;
        }

        if(controller.keyDictionary["Down"]){
            if(currentSpeed==baseSpeed && !isLower){ // le faire baissé 
                animator.SetTrigger("lower");
                isLower=true;
            }else{ // Faire le Spindash
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
        if(SonicActions.instance.objs.Count > 0){
            //Debug.Log("Nom de l'objet le plus proche :"+FindNearEnenmy().name);

            if(!MoveHoming){ // Si le mouv du homing attack ne se fait pas

                targeted=FindNearEnenmy();

                if(targeted!=null && isJumping && homingAttackPossible && Input.GetKeyDown(KeyCode.Space) && !isfalling ){
                    MoveHoming=true;
                    
                }
                
            }

            
        }


        // Animation        

        animator.SetBool("isGrounded",isGrounded);
        animator.SetBool("isjumping",isJumping);
        animator.SetFloat("speed",currentSpeed);
        animator.SetBool("isfalling",isfalling);
        animator.SetBool("SpinDash",SpinDash);
        
        if(currentSpeed>baseSpeed){
            animator.SetBool("baseSpeed",true);
        }else{
            animator.SetBool("baseSpeed",false);
        }
        
    }

    void FixedUpdate(){
        // Code de la physique ici

        if(MoveHoming){
            HomingAttack();
        }
        if(MoveJump){
             Jump(jumpForce);
             MoveJump=false;
        }

        Flip(horizonMov);
        Move(horizonMov);
    }

    void DetectSideCollisions()
    {
        //Detecter si sonic est sur le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,collisionGroundLayer );

      
       // Longueur des rayons égale à la hauteur de l'objet
        float rayLength = boxCollider.size.y;

        // Point de départ du rayon à gauche (au bord du collider)
        Vector2 leftRayOrigin = new Vector2(boxCollider.bounds.min.x - xOffsetWallCollision, boxCollider.bounds.center.y);

        // Point de départ du rayon à droite (au bord du collider)
        Vector2 rightRayOrigin = new Vector2(boxCollider.bounds.max.x + xOffsetWallCollision, boxCollider.bounds.center.y);

        // Raycast vertical vers le haut depuis la gauche
         hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.up, rayLength, collisionWallMask);
        if (hitLeft.collider != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            currentSpeed = baseSpeed;
        }

        // Raycast vertical vers le haut depuis la droite
         hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.up, rayLength, collisionWallMask);
        if (hitRight.collider != null)
        {
           rb.velocity = new Vector2(0, rb.velocity.y);
           currentSpeed = baseSpeed;
        }

        // Dessiner les rayons dans la vue de la scène pour le débogage
        Debug.DrawRay(leftRayOrigin, Vector2.up * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.up * rayLength, Color.red);

    }

    public void Move(float _horizon){
        
        // Si un mur rencontrer currentSpeed=0;



        //Application de l'inertie en fonction de certaine condition
        //LA condition c'est si il n'appuie sur les boutons / si il fait un spindahs ou il change de cote
        if ((!controller.keyDictionary["Right"] && !controller.keyDictionary["Left"]) || InirtiaPossible || SpinDash )
        {
            Inertia = currentSpeed *1.5f;
            if(currentSpeed>baseSpeed){ // Appliquer l'inertie si la vitesse courante est superieur a celui de base 
                currentSpeed -= (Inertia * Time.deltaTime);
            }else if(currentSpeed<baseSpeed){
                currentSpeed = baseSpeed;         
            }

            TrueSpeed=lastSpeedDirection*(currentSpeed-baseSpeed);
            rb.velocity = new Vector2(TrueSpeed, rb.velocity.y); 
                
            if(currentSpeed==baseSpeed && InirtiaPossible){
                InirtiaPossible=false;
                animator.SetTrigger("unflip");
            }

               
            
        }else{ // Mouvement "Classique"

            if(horizonMov<0){
                lastSpeedDirection=-1;
            }else{
                lastSpeedDirection=1;
            }

            if (currentSpeed < maxSpeed && isGrounded)
            {
                currentSpeed += (speedIncreaseRate * Time.deltaTime);
            }

            TrueSpeed=currentSpeed*lastSpeedDirection;
            rb.velocity = new Vector2(TrueSpeed, rb.velocity.y); 
        }

       
    }

    public void Jump(float jumpForce){
        
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        //rb.AddForce(Vector2.left * rb.velocity.x * airResistance, ForceMode2D.Impulse); 
    }

    public void HomingAttack(){
        
        if (targeted!=null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targeted.transform.position, homingSpeed * Time.deltaTime);
        }else{
            MoveHoming = false;
        }
        
        homingAttackPossible=false;
        
    }
    
    public void TakeDamage(GameObject collision){
        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        transform.Translate(knockbackDirection * -knockbackDistance);
        Debug.Log("RECUL");
    }
    

    void Flip(float _horizon){
        
        if(_horizon>0.1f){
            if(spriteRenderer.flipX){
                 fliped=true;
            }else{
                fliped=false;
            }
            spriteRenderer.flipX=false;
            
        }else if(_horizon<-0.1f){
            if(!spriteRenderer.flipX){
                 fliped=true;
            }else{
                fliped=false;
            }
            spriteRenderer.flipX=true;
            
        }

        if(fliped && currentSpeed>baseSpeed+offsetFlipChange){
            InirtiaPossible=true;
            animator.SetTrigger("flip");
        }
        
    }

    private void OnDrawGizmos(){
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }

    
    public GameObject FindNearEnenmy(){
        
        GameObject objProche=null;
        
        // chercher l'ennemi plus proche avec la distance 
        foreach(GameObject obj in SonicActions.instance.objs) {

            if(objProche==null && !Obstacle(obj) ){
                objProche=obj;
            }else if (objProche!=null){
                if(Vector3.Distance(gameObject.transform.position, obj.transform.position) < Vector3.Distance(gameObject.transform.position, objProche.transform.position) && !Obstacle(obj)) {
                    objProche=obj;
                }
            }
        }

       return objProche; 
    }

    public bool Obstacle(GameObject pointDArrivee){
        Vector3 positionDepart = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        /*
        lineRenderer.SetPosition(0,positionDepart);
        lineRenderer.SetPosition(1, pointDArrivee.transform.position);
        */
         RaycastHit2D hitInfo = Physics2D.Linecast(positionDepart, pointDArrivee.transform.position,~layersToIgnoreHomingAttack);
         
         if (hitInfo.collider != null ){
           //Debug.Log("objet entre en colision : "+hitInfo.collider.gameObject.name);
            return true;
         }else{
           // Debug.Log("PAS DE COLLISION");
            return false;
         }
         
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifier si la collision vient de la gauche ou de la droite
         foreach (ContactPoint2D contact in collision.contacts)
        {
            
        }
    }

    
}
