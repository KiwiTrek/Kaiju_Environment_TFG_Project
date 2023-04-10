using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    public BossSubHitbox victoryChecker = null;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;

        if (victoryChecker.currentHits == victoryChecker.maxLives)
        {
            SceneManager.LoadScene(1);
        }
    }
}
