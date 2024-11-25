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
    Rigidbody2D rb;
    Animator animator;
    Collider2D playerCollider;
    private float dirX; // Controlled by AI
    private bool jump; // Controlled by AI
    private enum MovementState { idle, running, jumping, falling };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the player and environment
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero; // Adjust for your spawn location
        gameState.ResetState();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add player position and velocity as observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rb.velocity);

        // Include game state observations (like onIce, wingsHit, etc.)
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
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveDirection = actions.DiscreteActions[0]; // 0: Left, 1: Right, 2: Jump

        switch (moveDirection)
        {
            case 0: // Move left
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                break;
            case 1: // Move right
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                break;
            case 2: // Jump
                if (IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                }
                break;
        }

        // Collecting coins
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var coin in coins)
        {
            if (Vector2.Distance(transform.position, coin.transform.position) < .1f)
            {
                AddReward(1.0f);
            }
        }
    }
    private bool IsGrounded()
    {
        // Check if the player is grounded to allow jumping
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
    }

    void MovePlayer()
    {
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (gameState.onIce)
        {
            rb.velocity = new Vector2(dirX * moveSpeed * 1.5f, rb.velocity.y);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Trampoline"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 1.5f * jumpSpeed);
        }

        if (collision.gameObject.name == "Ice")
        {
            gameState.onIce = true;
        }

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Bottom"))
        {
            Die();
        }
    }
    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("death");

        // Penalize the agent for dying.
        AddReward(-1.0f);

        // End the episode.
        EndEpisode();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.name == "Ice")
        {
            gameState.onIce = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            CoinCollectionSound.Play();
        }

        if (collision.gameObject.CompareTag("FinalCheckpoint"))
        {
            FinalCheckpointSound.Play();
            AddReward(5.0f); // Large reward for completing the level
            EndEpisode();
        }
    }
    
}
