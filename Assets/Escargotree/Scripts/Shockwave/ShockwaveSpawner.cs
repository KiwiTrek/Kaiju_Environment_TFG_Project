using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ShockwaveSpawner : MonoBehaviour
{
    public MeshRenderer rendererEmissive;
    public GameObject shockwavePrefab;
    public GameObject electricityPrefab;
    public float spawnRate;
    public float finalHeightMultiplier = 0.666666667f;
    public AudioSource charging;
    public AudioSource shockwave;
    float spawnTime;
    GameObject currentZap=null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rendererEmissive.material.SetFloat("_Fill", spawnTime);
        spawnTime += Time.deltaTime;
        if (spawnTime < 0.0f)
        {
            charging.Pause();
        }
        else
        {
            charging.UnPause();
        }
        if (spawnTime > spawnRate)
        {
            if (currentZap != null) { Destroy(currentZap); }
            spawnTime = -(spawnRate/2.0f);
            GameObject wave = Instantiate(shockwavePrefab, this.transform);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            charging.Play();
            shockwave.Play();
            currentZap = Instantiate(electricityPrefab, this.transform);
        }
    }
}
