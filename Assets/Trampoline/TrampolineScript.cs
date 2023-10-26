using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineScript : MonoBehaviour
{
    Animator animator;
 
    private enum TrampolineState { idle, hit};
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrampolineState state;
        if (collision.gameObject.tag == "Player")
        {   state = TrampolineState.hit;
            animator.SetInteger("state", (int)state);
            
            
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        TrampolineState state;
        if(collision.gameObject.tag == "Player")
        {
            state = TrampolineState.idle;
            animator.SetInteger("state", (int)state);
        }
    }
}
