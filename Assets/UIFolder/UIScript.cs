using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    GameState gameState;
    [SerializeField]
    public TextMeshProUGUI CoinsText;
    [SerializeField]
    public TextMeshProUGUI GoBackText;
    [SerializeField]
    public TextMeshProUGUI WinningText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        CoinsText.text = "Coins Collected: " + gameState.coinsCollected.ToString() + " / " + gameState.checkpointNum;
        if(gameState.coinsCollected < gameState.checkpointNum)
        {
            CoinsText.color = Color.red;
        }
        else CoinsText.color = Color.green;

        if(gameState.checkpointNum == 11)
        {
            CoinsText.text = "GO FOR GOLD";
            CoinsText.color = Color.yellow;
        }

        if (gameState.displayCheckpoint && gameState.checkpointTouched)
        {
            GoBackText.text = "Go Back and Grab Coins";
            StartCoroutine(waitForUI(2.0f));
        }
        else if(gameState.displayCheckpoint)
        {
            GoBackText.text = "Walk into house to buy: " + gameState.gobNames[gameState.checkpointNum].ToString() + " gob";
            StartCoroutine(waitForUI(2.0f));
        }
        if (gameState.finalCheckpointHit)
        {
            WinningText.text = "IM SORRY FOR NOT BUYING GOBS";
            StartCoroutine(waitForUI2(5.0f));
        }
    }

    IEnumerator waitForUI (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GoBackText.text = " ";
        GoBackText.text = " ";
        gameState.checkpointTouched = false;
    }

    IEnumerator waitForUI2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        WinningText.text = " ";
        gameState.finalCheckpointHit = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
