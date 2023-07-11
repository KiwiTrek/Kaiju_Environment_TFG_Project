using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    public List<GameObject> doors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            gameObject.SetActive(false);

            //Effects

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
        //Effects (Door)

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
    }
}
