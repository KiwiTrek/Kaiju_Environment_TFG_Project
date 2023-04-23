using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public TMP_Text missionText = null;
    public CameraSwitcher cameraSwitcher = null;
    
    [Header("Game Objects in Scene")]
    public BossSubHitbox victoryChecker = null;
    public GameObject spawnPoint = null;
    public CharacterLives lives = null;
    public BossMov bossMov = null;

    [Space(10)]
    public DataCompilator compilator= null;
    void Start()
    {
        missionText.text = "Current objective: \nFind Escargotree!";
    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;
        if (lives == null) return;
        if (spawnPoint == null) return;
        if (bossMov == null) return;

        if (Vector3.Distance(lives.gameObject.transform.position, spawnPoint.transform.position) <= 5.0f)
        {
            missionText.text = "Current objective: \nTrip him up!";
        }

        if (bossMov.legsDestroyed >= 3 || bossMov.isDown)
        {
            missionText.text = "Current objective: \nClimb to the top!";
        }

        if (cameraSwitcher.id == 3)
        {
            missionText.text = "Current objective: \nDefeat Carpintroyer!";
        }

        if (lives.dead && lives.deathCounter >= 3.0f)
        {
            cameraSwitcher.id = 0;
        }

        if (victoryChecker.currentHits == victoryChecker.maxLives)
        {
            if (compilator != null)
            {
                compilator.EndSession(DateTime.Now);
            }

            SceneManager.LoadScene(1);
        }
    }
}
