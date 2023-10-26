using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobMovement : MonoBehaviour
{
    
    // Start is called before the first frame update
    
    void Start()
    {
        Physics2D.IgnoreLayerCollision(6, 7);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((-5f * Time.deltaTime), 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Death")
        {
            Destroy(this.gameObject);
        }
        if(collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }

}
