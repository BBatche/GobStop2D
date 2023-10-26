using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState.coinsCollected = 0;
        gameState.checkpointNum = 1;
        gameState.checkpointTouched = false;
        gameState.displayCheckpoint = false;
        gameState.onIce = false;
        gameState.wingsHit = false;
        gameState.glueHit = false;
        gameState.cowDrinkHit= false;
        gameState.finalCheckpointHit= false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
