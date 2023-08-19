using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraSwitcher switcher;
    public BossSubHitbox bossLife;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (tag == "2DZoneCam")
            {
                switcher.id = 1;
            }
            else if (tag == "LegZoneCam")
            {
                switcher.id = 2;
            }
            else if (tag == "BossZoneCam")
            {
                switcher.id = 3;
            }
            else
            {
                switcher.id = 0;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (tag == "2DZoneCam")
            {
                switcher.id = 1;
            }
            else if (tag == "LegZoneCam")
            {
                switcher.id = 2;
            }
            else if (tag == "BossZoneCam")
            {
                switcher.id = 3;
            }
            else
            {
                switcher.id = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Returning to status");
            switcher.id = 0;
        }
    }
}
