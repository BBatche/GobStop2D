using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;
    [SerializeField]
    PhysicsMaterial2D slippery;
    [SerializeField]
    PhysicsMaterial2D bouncy;
    [SerializeField]
    AudioSource CoinCollectionSound;
    [SerializeField]
    AudioSource FinalCheckpointSound;
    bool isGrounded = false;
    bool spaceBarDown = false;
    float jumpSpeed = 15.0f;
    float moveSpeed = 7.0f;
    float icyMoveSpeed = 10f;
    Rigidbody2D rb;
    Animator animator;
    Collider2D playerCollider;
    float dirX;
    float dirY;
    private enum MovementState {idle, running, jumping, falling };
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameState.onIce);
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceBarDown = true;
        }
        UpdateAnimation();


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
        if(collision.gameObject.tag == "Trampoline")
        {
            rb.velocity = new Vector3(rb.velocity.x, 1.5f * jumpSpeed, 0);
        }
        if(collision.gameObject.name == "Ice")
        {
            gameState.onIce = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
        if (collision.gameObject.name == "Ice")
        {
            gameState.onIce = false;
        }
    }
    private void FixedUpdate()
    {
        if (spaceBarDown)
        {
            Debug.Log("Space down");
            spaceBarDown = false;
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, 0);
            }
        }

        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");


        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        

        
        if (gameState.onIce) 
        {
            dirX = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(dirX * moveSpeed * 1.5f, rb.velocity.y);
            
            if(dirX > 0)
            {
                rb.AddForce(new Vector2(.5f, 0f));
            }
            else if(dirX < 0) 
            {
                rb.AddForce(new Vector2(-.5f, 0f));
            }
        }
        else
        {
            dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        }

        if (gameState.wingsHit)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);
            StartCoroutine(WingTimer(5.0f));
        }

        if (gameState.glueHit)
        {
            rb.velocity = new Vector2(dirX * moveSpeed * .5f, rb.velocity.y);
            StartCoroutine(GlueTimer(5.0f));
        }

        if (gameState.cowDrinkHit)
        {
            playerCollider.sharedMaterial.bounciness = 1;
            StartCoroutine(CowDrinkTimer(5.0f));
            
        }


        if (dirX < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }
        else
        {
            transform.eulerAngles = new Vector2(0, 0);
        }
    }
    private void UpdateAnimation()
    {
        MovementState state;
        if (dirX == 0)
        {
            state = MovementState.idle;
        }
        else
        {
            state = MovementState.running;
        }

        if (rb.velocity.y >.1f)
        {
            state = MovementState.jumping;
        }
        else if(rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    IEnumerator WingTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameState.wingsHit = false;
    }
    IEnumerator GlueTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameState.glueHit = false;
    }

    IEnumerator CowDrinkTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameState.cowDrinkHit = false;
        playerCollider.sharedMaterial.bounciness = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            CoinCollectionSound.Play();
        }
        if(collision.gameObject.tag == "FinalCheckpoint")
        {
            FinalCheckpointSound.Play();
        }
    }
}
