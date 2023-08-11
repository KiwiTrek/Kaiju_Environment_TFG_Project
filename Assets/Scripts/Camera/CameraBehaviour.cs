using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 initialPosition = Vector3.zero;
    public GameObject objective;
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (objective != null)
        {
            //cameraLevel.transform.localPosition = new Vector3(0.273999989f, 0.00300000003f, 0);
            //cameraLeg.transform.localPosition = new Vector3(0f, 0.0661000013f, 0f);

            objective.transform.localPosition = initialPosition;
            Vector3 currentPosition = objective.transform.position;
            currentPosition.y = player.transform.position.y;
            objective.transform.position = currentPosition;
            objective.transform.LookAt(player.transform);
        }
    }
}
