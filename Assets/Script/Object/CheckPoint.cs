using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Transform playerspawn;
    private Animator animator;
    void Awake()
    {
        playerspawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision){
     
        
        if(collision.gameObject.CompareTag("Player")){
            playerspawn.position = transform.position;
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("CheckPointTaken");
        }

        
    
    }


}
