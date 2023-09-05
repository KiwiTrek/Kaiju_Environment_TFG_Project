using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoss : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayDirector.cutsceneMode == CutsceneType.BirdIntro)
        {
            transform.LookAt(player.transform.position);
        }
        else
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, transform.localPosition.y, transform.localPosition.z);
            transform.LookAt(targetPosition);
        }
    }
}
