using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public bool is2D = true; //True: 2D
    public GameObject objective;
    public GameObject player;

    float xOffset = 0.0f;

    // Update is called once per frame

    private void Start()
    {
        xOffset = transform.localPosition.x;
    }
    void Update()
    {
        if (objective != null)
        {
            if (is2D)
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
