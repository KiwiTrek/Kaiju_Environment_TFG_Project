using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoss : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public float cameraSpeed = 1.0f;
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
            Vector3 playerDirection = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerDirection), Time.deltaTime * cameraSpeed);
        }
    }
}
