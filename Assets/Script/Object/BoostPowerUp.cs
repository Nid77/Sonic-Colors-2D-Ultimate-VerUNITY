
using UnityEngine;

public class BoostPowerUp : MonoBehaviour
{

     public int amountBoost;
     private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Player")){
            Destroy(gameObject);
           SonicBoost.instance.takeBoost(12);
            
        }
    }

    
}
