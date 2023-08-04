using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Sprites;
using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    public List<GameObject> keyMeshes;
    public List<GameObject> doors;
    public GameObject spawnVFX;
    public GameObject keyVFX;
    public AudioSource keyAudio;

    float timer = 0.0f;
    bool picked = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (picked)
        {
            timer += Time.deltaTime;
            if (timer > 1.5f)
            {
                gameObject.SetActive(false);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            foreach (GameObject go in keyMeshes)
            {
                go.SetActive(false);
            }
            picked = true;

            keyAudio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            keyAudio.Play();

            GameObject vfx = Instantiate(keyVFX, spawnVFX.transform);
            Destroy(vfx, 0.5f);

            ActivateDoors(doors);
        }
    }

    private void ActivateDoors(List<GameObject> doors)
    {
        foreach(GameObject door in doors)
        {
            door.GetComponent<MeshCollider>().enabled = false;
            if (door.GetComponent<BoxCollider>() != null)
            {
                door.GetComponent<BoxCollider>().enabled = true;
            }
            if (door.GetComponentInChildren<Animator>() != null)
            {
                door.GetComponentInChildren<Animator>().SetBool("isOpen", true);
            }
        }

    }

    public void Restart()
    {
        foreach(GameObject door in doors)
        {
            door.GetComponent<MeshCollider>().enabled = true;
            if (door.GetComponent<BoxCollider>() != null)
            {
                door.GetComponent<BoxCollider>().enabled = false;
            }
            if (door.GetComponentInChildren<Animator>() != null)
            {
                door.GetComponentInChildren<Animator>().SetBool("isOpen", false);
            }
        }

        gameObject.SetActive(true);
        foreach (GameObject go in keyMeshes)
        {
            go.SetActive(false);
        }
        picked = false;
        timer = 0.0f;

    }
}
