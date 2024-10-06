using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            AudioManager.instance.PlayClipAt(sound,transform.position);
            UI.instance.AddRing(1);
            Destroy(gameObject);
        }
    }

}
