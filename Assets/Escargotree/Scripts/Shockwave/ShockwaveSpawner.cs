using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveSpawner : MonoBehaviour
{
    public GameObject shockwavePrefab;
    public float spawnRate;
    float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime > spawnRate)
        {
            spawnTime = 0;
            Instantiate(shockwavePrefab, this.transform);
        }
    }
}
