using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    public BossSubHitbox victoryChecker = null;
    public CharacterLives lives = null;
    public DataCompilator compilator= null;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;
        if (lives == null) return;

        if (lives.dead && lives.deathCounter >= 3.0f)
        {
            this.GetComponent<CameraSwitcher>().id = 0;
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
