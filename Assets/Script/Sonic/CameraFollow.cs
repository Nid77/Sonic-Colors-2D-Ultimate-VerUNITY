
using UnityEngine;


public class CameraFollow : MonoBehaviour
{

    public GameObject sonic;
    public float timeOffset=0.2f;
    public Vector3 posOffset=new Vector3(0f,0f,-10f);
    private Vector3 velocity;
    public bool IsAlive=true;
    private float coef=1f;
    public static CameraFollow instance;

    private void Awake(){
        if(instance!=null){
            Debug.Log("Plus d'une instance de SonicMovement dans la scene");
        }

        instance = this;
    }
    void Update()
    {   
        coef=SonicMovement.instance.lastSpeedDirection;
        posOffset.x=(SonicMovement.instance.currentSpeed-SonicMovement.instance.baseSpeed)*coef/5;
        if(IsAlive){
            transform.position = Vector3.SmoothDamp(transform.position,sonic.transform.position+posOffset ,ref velocity,timeOffset);
        }
        
    }
}
