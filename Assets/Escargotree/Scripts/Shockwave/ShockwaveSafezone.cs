using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveSafezone : MonoBehaviour
{
    public bool isSafe = false;
    public void Update()
    {}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isSafe = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isSafe = true;
        }
    }
}
