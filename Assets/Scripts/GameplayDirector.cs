using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CutsceneType
{
    None,
    BossIntro,
    BossFirstPhaseEnd,
    JumpToBoss,
    FirstDoorOpen,
    BirdIntro,
    BirdEnd
}

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public GameObject missionPanel;
    public TMP_Text missionText = null;
    public CameraSwitcher cameraSwitcher = null;
    public AudioSource music = null;
    public AudioSource noticeMeAudio = null;
    public AudioSource chanChan = null;
    public Image noticeMe = null;
    public float maxTimeNoticeMe = 2.0f;
    public float betweenTimeNoticeMe = 0.25f;
    float timer = 0.0f;
    float timerBetween = 0.0f;
    public TMP_Text prevText = null;

    [Space]
    [Header("Cutscenes")]
    public GameObject cutsceneBars = null;
    public static CutsceneType cutsceneMode = CutsceneType.None;

    [Space(5)]
    float timerFirstCutscene = 0.0f;
    public GameObject camFirstPhase1 = null;
    public GameObject camFirstPhase2 = null;

    [Space(5)]
    float timerDoorCutscene = 0.0f;
    public GameObject camDoorPhase = null;
    CinemachineVirtualCamera camDoorScript = null;
    CinemachineTrackedDolly camDoorDolly = null;
    public DoorConnection doorFirstCutscene = null;

    [Space(5)]
    float timerBirdTantrum = 0.0f;
    public GameObject camBirdBoss = null;
    CinemachineVirtualCamera camBirdScript = null;
    CinemachineTrackedDolly camBirdDolly = null;


    [Space]
    [Header("Music")]
    public AudioClip wind = null;
    public AudioClip first = null;
    public AudioClip second = null;
    public AudioClip third = null;
    AudioClip previous = null;

    [Space]
    [Header("Game Objects in Scene")]
    public BossSubHitbox victoryChecker = null;
    BossSubBehaviour bossSubBehaviour = null;
    public BossMov bossMov = null;
    public GameObject spawnPoint = null;
    public CharacterLives lives = null;
    public Material debugColliderMaterial = null;

    [Space(10)]
    public DataCompilator compilator= null;
    void Start()
    {
        missionText.text = "Current objective: \nFind Escargotree!";
        prevText.text = missionText.text;
        Color color = debugColliderMaterial.color;
        color.a = 0;
        debugColliderMaterial.color = color;
        music.clip = wind;
        previous = wind;
        music.Play();
        camDoorScript = camDoorPhase.GetComponent<CinemachineVirtualCamera>();
        camDoorDolly = camDoorScript.GetCinemachineComponent<CinemachineTrackedDolly>();
        camBirdScript = camBirdBoss.GetComponent<CinemachineVirtualCamera>();
        camBirdDolly = camBirdScript.GetCinemachineComponent<CinemachineTrackedDolly>();
        bossSubBehaviour = victoryChecker.GetComponent<BossSubBehaviour>();
        timer = -0.5f;
        timerBetween = 0.0f;
        timerFirstCutscene = 0.0f;
        timerDoorCutscene = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;
        if (lives == null) return;
        if (spawnPoint == null) return;
        if (bossMov == null) return;

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

        if (cutsceneMode == CutsceneType.None)
        {
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
        }


        if (prevText.text != missionText.text)
        {
            prevText.text = missionText.text;
            if (prevText.text == "Current objective: \nTumble it down!")
            {
                //Intro Cutscene
                chanChan.Play();
            }
            else if (prevText.text == "Current objective: \nClimb to the top!")
            {
                //Leg Cutscene
                cutsceneMode = CutsceneType.BossFirstPhaseEnd;
            }
            else if (prevText.text == "Current objective: \nDefeat Carpintroyer!")
            {
                //Intro Subboss
                cutsceneMode = CutsceneType.BirdIntro;
                music.Pause();
                chanChan.Play();
            }
            timer = 0.0f;
        }

        if (cutsceneMode == CutsceneType.None)
        {
            if (previous != music.clip)
            {
                previous = music.clip;
                music.Play();
            }
        }

        if (Vector3.Distance(lives.gameObject.transform.position, spawnPoint.transform.position) <= 15.0f)
        {
            missionText.text = "Current objective: \nTumble it down!";
            music.clip = first;
            bossMov.canvas.SetActive(true);
        }

        if (bossMov.legsDestroyed >= 3 || bossMov.isDown)
        {
            missionText.text = "Current objective: \nClimb to the top!";
            music.clip = second;
        }

        if (cameraSwitcher.id == 1 || cameraSwitcher.id == 2)
        {
            music.clip = second;
        }

        if (cameraSwitcher.id == 3)
        {
            missionText.text = "Current objective: \nDefeat Carpintroyer!";
            music.clip = third;
        }

        if (lives.dead && lives.deathCounter >= 3.0f)
        {
            cameraSwitcher.id = lives.cameraID;
            if (victoryChecker != null)
            {
                victoryChecker.currentHits = 0;
            }
        }

        if (victoryChecker.currentHits >= victoryChecker.maxLives)
        {
            cutsceneMode = CutsceneType.BirdEnd;
        }

        //Cutscenes
        if (doorFirstCutscene.activateCutscene)
        {
            doorFirstCutscene.activateCutscene = false;
            cutsceneMode = CutsceneType.FirstDoorOpen;
        }

        if (cutsceneMode == CutsceneType.None || cutsceneMode == CutsceneType.JumpToBoss)
        {
            missionPanel.SetActive(true);
            cutsceneBars.SetActive(false);
        }
        else
        {
            missionPanel.SetActive(false);
            cutsceneBars.SetActive(true);
        }

        switch (cutsceneMode)
        {
            case CutsceneType.None:
                break;
            case CutsceneType.BossIntro:
                music.Stop();
                break;
            case CutsceneType.BossFirstPhaseEnd:
                {
                    if (bossMov.switchToSecondCam)
                    {
                        camFirstPhase1.SetActive(false);
                        camFirstPhase2.SetActive(true);
                        timerFirstCutscene += Time.deltaTime;
                        if (timerFirstCutscene >= 5.0f)
                        {
                            camFirstPhase1.SetActive(false);
                            camFirstPhase2.SetActive(false);
                            timerFirstCutscene = 5.0f;
                            cutsceneMode = CutsceneType.None;
                        }
                    }
                    else
                    {
                        camFirstPhase1.SetActive(true);
                        camFirstPhase2.SetActive(false);
                    }
                    break;
                }
            case CutsceneType.JumpToBoss:
                break;
            case CutsceneType.FirstDoorOpen:
                {
                    camDoorPhase.SetActive(true);
                    camDoorDolly.m_PathPosition += Time.deltaTime * 2.5f;
                    if (camDoorDolly.m_PathPosition >= 4.0f)
                    {
                        doorFirstCutscene.ActivateDoors(doorFirstCutscene.doors);
                        timerDoorCutscene += Time.deltaTime;
                    }

                    if (timerDoorCutscene >= 2.0f)
                    {
                        camDoorPhase.SetActive(false);
                        cutsceneMode = CutsceneType.None;
                    }
                    break;
                }
            case CutsceneType.BirdIntro:
                {
                    camBirdBoss.SetActive(true);
                    camBirdDolly.m_PathPosition += Time.deltaTime * 1.5f;
                    bossSubBehaviour.gameObject.transform.LookAt(lives.transform.position);
                    if (camBirdDolly.m_PathPosition >= 5.0f)
                    {
                        bossSubBehaviour.SwitchAnimation(CurrentAnimation.Tantrum);
                        timerBirdTantrum += Time.deltaTime;
                    }
                    if (timerBirdTantrum >= 2.0f)
                    {
                        camBirdBoss.SetActive(false);
                        cutsceneMode = CutsceneType.None;
                    }
                    break;
                }
            case CutsceneType.BirdEnd:
                {
                    if (compilator != null)
                    {
                        compilator.EndSession(DateTime.Now);
                    }
                    Debug.Log("You win!");
                    SceneManager.LoadScene(1);

                    break;
                }
            default:
                break;
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
