using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "State/GobStopGameState")]
public class GameState : ScriptableObject
{
    public int coinsCollected { get; set; }

    public int checkpointNum { get; set; }

    public bool checkpointTouched { get; set; }

    public bool displayCheckpoint { get; set; }

    public bool onIce { get; set; }

    public bool wingsHit { get; set; }

    public bool glueHit { get; set; }

    public bool cowDrinkHit { get; set; }

    public bool finalCheckpointHit { get; set; }

    public string[] gobNames = { "neopolitan", "blueberry", " red velvet", "strawberry", "orange", "pumpkin", "carrot", "banana ", "chocolate", "carolina reaper" };

}
