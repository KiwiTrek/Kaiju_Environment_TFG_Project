using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorConnection : MonoBehaviour
{
    public GameObject door;

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

            ActivateDoor(door);
        }
    }

    private void ActivateDoor(GameObject door)
    {
        //Effects (Door)

        door.SetActive(false);
    }

    public void Restart()
    {
        gameObject.SetActive(true);
        door.SetActive(true);
    }
}
