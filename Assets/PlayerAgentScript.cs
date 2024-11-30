using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class PlayerAgentScript : Agent
{
    [SerializeField] GameState gameState;
    [SerializeField] PhysicsMaterial2D slippery;
    [SerializeField] PhysicsMaterial2D bouncy;
    [SerializeField] AudioSource CoinCollectionSound;
    [SerializeField] AudioSource FinalCheckpointSound;

    bool isGrounded = false;
    float jumpSpeed = 15.0f;
    float moveSpeed = 7.0f;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    Animator animator;
    [SerializeField]
    Collider2D playerCollider;
    private float dirX; // Controlled by AI
    private bool jump; // Controlled by AI
    private enum MovementState { idle, running, jumping, falling };

    bool grounded;
    public override void Initialize()
    {

        Debug.Log("Starting PlayerAgentScript...");
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Beginning...");
        // Reset the player and environment
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero; // Adjust for your spawn location
        gameState.ResetState();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("collecting observations...");
        //Add player position and velocity as observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rb.velocity);

        //include game state observations(like onice, wingshit, etc.)
        sensor.AddObservation(gameState.onIce ? 1.0f : 0.0f);
        sensor.AddObservation(gameState.wingsHit ? 1.0f : 0.0f);
        sensor.AddObservation(gameState.glueHit ? 1.0f : 0.0f);

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var coin in coins)
        {
            sensor.AddObservation(coin.transform.position);
        }

        foreach (var enemy in enemies)
        {
            sensor.AddObservation(enemy.transform.position);
        }

        Debug.Log("total observations: " + sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Action received : ");
        int moveDirection = actions.DiscreteActions[0]; // 0: Left, 1: Right
        bool isJumping = actions.DiscreteActions[1] == 1; // 1: Jump, 0: No Jump

        // Move the player
        switch (moveDirection)
        {
            case 0: // Move left
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                break;
            case 1: // Move right
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                AddReward(1.0f);
                break;
        }

        // Handle jumping
        if (isJumping && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // Collecting coins
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var coin in coins)
        {
            if (Vector2.Distance(transform.position, coin.transform.position) < .1f)
            {
                Debug.Log("Coin collected!");
                AddReward(100.0f);
            }
        }
    }
    void FixedUpdate()
    {
        // Request decision after collecting observations
        RequestDecision();
    }
    private bool IsGrounded()
    {
        // Check if the player is grounded to allow jumping
        
        if (grounded)
        {
            Debug.Log("Player is grounded");
        }
        else
        {
            Debug.Log("Player is not grounded");
        }
        return grounded;
    }

    void MovePlayer()
    {
        if (jump && isGrounded)
        {
            Debug.Log("Performing jump...");
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (gameState.onIce)
        {
            rb.velocity = new Vector2(dirX * moveSpeed * 1.5f, rb.velocity.y);
            Debug.Log("Player is on ice, increasing speed...");
        }

        UpdateAnimation();
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

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
        Debug.Log("Current animation state: " + state.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            Debug.Log("Player grounded.");
        }

        if (collision.gameObject.CompareTag("Trampoline"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 1.5f * jumpSpeed);
            Debug.Log("Player bounced on trampoline.");
        }

        if (collision.gameObject.name == "Ice")
        {
            gameState.onIce = true;
            Debug.Log("Player hit ice.");
        }

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Bottom"))
        {
            AddReward(-1.0f);
            Debug.Log("Player died.");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died, triggering death animation...");
       
        EndEpisode();
        AddReward(-1000.0f);
        Debug.Log(GetCumulativeReward());
        animator.SetTrigger("death");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            Debug.Log("Player left the ground.");
        }

        if (collision.gameObject.name == "Ice")
        {
            gameState.onIce = false;
            Debug.Log("Player left ice.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            CoinCollectionSound.Play();
            Debug.Log("Coin collected, playing sound.");
            AddReward(1.0f);
        }

        if (collision.gameObject.CompareTag("FinalCheckpoint"))
        {
            
            FinalCheckpointSound.Play();
            Debug.Log("Final checkpoint reached, playing sound.");
            AddReward(5.0f); // Large reward for completing the level
            EndEpisode();
        }
    }
}