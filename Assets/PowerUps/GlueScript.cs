using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;
    private void OnTriggerEnter2D(Collider2D collision)
    {   if (collision.gameObject.tag == "Player")
        {
            gameState.glueHit = true;
            Destroy(this.gameObject);
        }
    }
}
