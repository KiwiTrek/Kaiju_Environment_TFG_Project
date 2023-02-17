using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public bool bossOrLevel = false; //Boss = false; Level = true
    public GameObject objective;
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (objective != null)
        {
            if (bossOrLevel)
            {
                Vector3 currentPosition = objective.transform.position;
                currentPosition.y = player.transform.position.y;
                objective.transform.position = currentPosition;
                objective.transform.LookAt(player.transform);
            }
            else
            {
                objective.transform.LookAt(player.transform);
            }
        }
    }
}
