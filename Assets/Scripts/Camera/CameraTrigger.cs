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
                Debug.Log("Entered 2D zone");
                switcher.id = 1;
            }
            else if (tag == "LegZoneCam")
            {
                Debug.Log("Entered Leg zone");
                switcher.id = 2;
            }
            else if (tag == "BossZoneCam")
            {
                Debug.Log("Entered boss zone");
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

        if (bossLife != null)
        {
            bossLife.currentHits = 0;
        }
    }
}
