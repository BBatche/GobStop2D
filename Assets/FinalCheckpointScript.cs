using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCheckpointScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameState.finalCheckpointHit = true;
    }
}
