using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GobSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject gobPrefab;
    [SerializeField]
    float spawnTime;
    public Vector3 position;
    IEnumerator SpawnGobAtRate(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            
            
            Instantiate(gobPrefab, position, Quaternion.identity);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnGobAtRate(spawnTime));
    }

    // Update is called once per frame
    void Update()
    {
        position = GameObject.FindGameObjectWithTag("GobSpawner").transform.position;
    }


}
