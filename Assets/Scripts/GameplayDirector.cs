using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public TMP_Text missionText = null;
    public CameraSwitcher cameraSwitcher = null;
    public AudioSource music = null;
    public AudioSource noticeMeAudio = null;
    public Image noticeMe = null;
    public float maxTimeNoticeMe = 2.0f;
    public float betweenTimeNoticeMe = 0.25f;
    float timer = 0.0f;
    float timerBetween = 0.0f;

    [Header("Music")]
    public AudioClip wind = null;
    public AudioClip first = null;
    public AudioClip second = null;
    public AudioClip third = null;
    
    [Header("Game Objects in Scene")]
    public BossSubHitbox victoryChecker = null;
    public GameObject spawnPoint = null;
    public CharacterLives lives = null;
    public BossMov bossMov = null;
    public Material debugColliderMaterial = null;

    [Space(10)]
    public DataCompilator compilator= null;
    AudioClip previous = null;
    void Start()
    {
        missionText.text = "Current objective: \nFind Escargotree!";
        Color color = debugColliderMaterial.color;
        color.a = 0;
        debugColliderMaterial.color = color;
        music.clip = wind;
        previous = wind;
        music.Play();
        timer = 0.0f;
        timerBetween = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;
        if (lives == null) return;
        if (spawnPoint == null) return;
        if (bossMov == null) return;

        if (timer < maxTimeNoticeMe)
        {
            timer += Time.deltaTime;
            timerBetween += Time.deltaTime;
            if (timerBetween >= betweenTimeNoticeMe)
            {
                timerBetween = 0.0f;
                Debug.Log("Hey! Listen! Timer: " + timer);
                if (!noticeMe.enabled)
                {
                    noticeMeAudio.Play();
                }
                noticeMe.enabled = !noticeMe.enabled;
            }
        }
        else
        {
            timer = maxTimeNoticeMe;
            timerBetween = 0.0f;
            noticeMe.enabled = false;
        }

        if (previous != music.clip)
        {
            previous = music.clip;
            music.Play();
        }

        if (Vector3.Distance(lives.gameObject.transform.position, spawnPoint.transform.position) <= 15.0f)
        {
            missionText.text = "Current objective: \nTumble it down!";
            music.clip = first;
            if (timer >= maxTimeNoticeMe && bossMov.canvas.activeSelf == false) timer = 0.0f;
            bossMov.canvas.SetActive(true);
        }

        if (bossMov.legsDestroyed >= 3 || bossMov.isDown)
        {
            missionText.text = "Current objective: \nClimb to the top!";
            music.clip = second;
            if (timer >= maxTimeNoticeMe) timer = 0.0f;
        }

        if (cameraSwitcher.id == 1 || cameraSwitcher.id == 2)
        {
            music.clip = second;
        }

        if (cameraSwitcher.id == 3)
        {
            missionText.text = "Current objective: \nDefeat Carpintroyer!";
            music.clip = third;
            if (timer >= maxTimeNoticeMe) timer = 0.0f;
        }

        if (lives.dead && lives.deathCounter >= 3.0f)
        {
            cameraSwitcher.id = lives.cameraID;
            if (victoryChecker != null)
            {
                victoryChecker.currentHits = 0;
            }
        }

        if (Input.GetKey(KeyCode.F2))
        {
            if (debugColliderMaterial != null)
            {
                Color color = debugColliderMaterial.color;
                if (color.a > 0)
                {
                    color.a = 0;
                }
                else
                {
                    color.a = 0.2f;
                }
                debugColliderMaterial.color = color;
            }
        }

        if (victoryChecker.currentHits >= victoryChecker.maxLives)
        {
            if (compilator != null)
            {
                compilator.EndSession(DateTime.Now);
            }
            Debug.Log("You win!");
            SceneManager.LoadScene(1);
        }
    }

    public void QuitApp()
    {
        Debug.Log("Quitting App!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
