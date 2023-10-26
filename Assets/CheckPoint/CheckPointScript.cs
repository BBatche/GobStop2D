using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameState.displayCheckpoint = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        gameState.displayCheckpoint = false;    
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (gameState.coinsCollected == gameState.checkpointNum)
            {
                gameState.checkpointTouched = true;
                
                GetComponent<Collider2D>().enabled = false;
                gameState.coinsCollected = 0;
                gameState.checkpointNum++;
                
            }
            else gameState.checkpointTouched = true;
                 gameState.displayCheckpoint = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }
}

