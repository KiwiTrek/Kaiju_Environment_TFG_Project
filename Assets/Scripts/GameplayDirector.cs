using Cinemachine;
using System;
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
    float timerNoticeMe = 0.0f;
    float timerBetween = 0.0f;
    public TMP_Text prevText = null;

    [Space]
    [Header("Cutscenes")]
    public GameObject cutsceneBars = null;
    public static CutsceneType cutsceneMode = CutsceneType.None;

    [Space(5)]
    public GameObject camIntro1 = null;
    public GameObject camIntro2 = null;
    CinemachineVirtualCamera camIntroScript2 = null;
    CinemachineBasicMultiChannelPerlin camIntroNoise2 = null;
    public Transform playerPosition = null;
    float timerBossIntro = 0.0f;

    [Space(5)]
    public GameObject camFirstPhase1 = null;
    public GameObject camFirstPhase2 = null;
    float timerFirstCutscene = 0.0f;

    [Space(5)]
    public GameObject camDoorPhase = null;
    CinemachineVirtualCamera camDoorScript = null;
    CinemachineTrackedDolly camDoorDolly = null;
    public DoorConnection doorFirstCutscene = null;
    float timerDoorCutscene = 0.0f;

    [Space(5)]
    public GameObject camBirdBoss = null;
    CinemachineVirtualCamera camBirdScript = null;
    CinemachineTrackedDolly camBirdDolly = null;
    float timerBirdTantrum = 0.0f;

    [Space(5)]
    public GameObject camBirdDeath = null;
    public Image fadeToBlack = null;
    public float timeBirdDeath = 5.0f;
    float currentTimerBirdDeath = 0.0f;
    bool startFadeOut = false;

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
        cutsceneMode = CutsceneType.None;
        music.Play();

        camDoorScript = camDoorPhase.GetComponent<CinemachineVirtualCamera>();
        camDoorDolly = camDoorScript.GetCinemachineComponent<CinemachineTrackedDolly>();
        camBirdScript = camBirdBoss.GetComponent<CinemachineVirtualCamera>();
        camBirdDolly = camBirdScript.GetCinemachineComponent<CinemachineTrackedDolly>();
        camIntroScript2 = camIntro2.GetComponent<CinemachineVirtualCamera>();
        camIntroNoise2 = camIntroScript2.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        bossSubBehaviour = victoryChecker.GetComponent<BossSubBehaviour>();

        timerNoticeMe = -0.5f;
        timerBetween = 0.0f;
        timerFirstCutscene = 0.0f;
        timerDoorCutscene = 0.0f;
        timerBossIntro = 0.0f;
        lives.spawnPoint = spawnPoint;

        camIntro1.SetActive(false);
        camIntro2.SetActive(false);
        camFirstPhase1.SetActive(false);
        camFirstPhase2.SetActive(false);
        camDoorPhase.SetActive(false);
        camBirdBoss.SetActive(false);
        camBirdDeath.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker == null) return;
        if (lives == null) return;
        if (spawnPoint == null) return;
        if (bossMov == null) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
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
#endif
        if (cameraSwitcher.id == 3)
        {
            missionText.text = "Current objective: \nDefeat Carpintroyer!";
            music.clip = third;
        }
        else if ((bossMov.legsDestroyed >= 3 || bossMov.isDown) && missionText.text != "Current objective: \nDefeat Carpintroyer")
        {
            missionText.text = "Current objective: \nClimb to the top!";
            music.clip = second;
        }
        else if (Vector3.Distance(lives.gameObject.transform.position, spawnPoint.transform.position) <= 15.0f)
        {
            missionText.text = "Current objective: \nTumble it down!";
            music.clip = first;
        }

        if (cameraSwitcher.id == 1 || cameraSwitcher.id == 2)
        {
            music.clip = second;
        }

        if (prevText.text != missionText.text)
        {
            prevText.text = missionText.text;
            if (prevText.text == "Current objective: \nTumble it down!")
            {
                //Intro Cutscene
                Debug.Log("Cutscene Mode: Intro");
                cutsceneMode = CutsceneType.BossIntro;
                Vector3 euler = bossMov.transform.eulerAngles;
                euler.y -= 180.0f;
                bossMov.transform.eulerAngles = euler;
                chanChan.Play();
            }

            if (prevText.text == "Current objective: \nClimb to the top!")
            {
                //Leg Cutscene
                Debug.Log("Cutscene Mode: First Phase Down");
                cutsceneMode = CutsceneType.BossFirstPhaseEnd;
            }

            if (prevText.text == "Current objective: \nDefeat Carpintroyer!")
            {
                //Intro Subboss
                Debug.Log("Cutscene Mode: Bird");
                cutsceneMode = CutsceneType.BirdIntro;
                music.Pause();
                chanChan.Play();
            }
            timerNoticeMe = 0.0f;
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
                {
                    if (!music.isPlaying && Time.timeScale != 0.0f)
                    {
                        music.Play();
                        Debug.Log("Play music! isPlaying: " + music.isPlaying);
                    }

                    if (previous != music.clip)
                    {
                        previous = music.clip;
                        Debug.Log("Current Music: " + music.clip.name);
                    }

                    if (timerNoticeMe < maxTimeNoticeMe)
                    {
                        timerNoticeMe += Time.deltaTime;
                        timerBetween += Time.deltaTime;
                        if (timerBetween >= betweenTimeNoticeMe)
                        {
                            timerBetween = 0.0f;
                            if (!noticeMe.enabled)
                            {
                                noticeMeAudio.Play();
                            }
                            noticeMe.enabled = !noticeMe.enabled;
                        }
                    }
                    else
                    {
                        timerNoticeMe = maxTimeNoticeMe;
                        timerBetween = 0.0f;
                        noticeMe.enabled = false;
                    }
                }
                break;
            case CutsceneType.BossIntro:
                {
                    if (music.isPlaying)
                        music.Pause();
                    if (bossMov.switchToSecondCam)
                    {
                        camIntro1.SetActive(false);
                        camIntro2.SetActive(true);
                        camIntroNoise2.m_AmplitudeGain -= Time.deltaTime * 3.0f;
                        if (camIntroNoise2.m_AmplitudeGain <= 0)
                        {
                            camIntroNoise2.m_AmplitudeGain = 0;
                        }
                        timerBossIntro += Time.deltaTime;
                        if (timerBossIntro >= 3.85f)
                        {
                            cutsceneMode = CutsceneType.None;
                            bossMov.canvas.SetActive(true);
                            bossMov.animator.SetInteger("cutsceneId", 0);
                            camIntro1.SetActive(false);
                            camIntro2.SetActive(false);
                            break;
                        }
                    }
                    else
                    {
                        bossMov.animator.SetInteger("cutsceneId", 1);
                        camIntro1.SetActive(true);
                        camIntro2.SetActive(false);
                    }
                    break;
                }
            case CutsceneType.BossFirstPhaseEnd:
                {
                    if (music.isPlaying)
                        music.Pause();
                    if (bossMov.switchToSecondCam)
                    {
                        camFirstPhase1.SetActive(false);
                        camFirstPhase2.SetActive(true);
                        timerFirstCutscene += Time.deltaTime;
                        if (timerFirstCutscene >= 3.0f)
                        {
                            camFirstPhase1.SetActive(false);
                            camFirstPhase2.SetActive(false);
                            timerFirstCutscene = 5.0f;
                            cutsceneMode = CutsceneType.None;
                            break;
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
                    if (music.isPlaying)
                        music.Pause();
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
                    if (music.isPlaying)
                        music.Stop();
                    camBirdDeath.SetActive(true);
                    startFadeOut = victoryChecker.animationFunctions.startFadeOutDeath;
                    if (startFadeOut)
                    {
                        currentTimerBirdDeath += Time.deltaTime;
                        Color tmp = fadeToBlack.color;
                        tmp.a = currentTimerBirdDeath / timeBirdDeath;
                        fadeToBlack.color = tmp;

                        if (currentTimerBirdDeath >= timeBirdDeath)
                        {
                            if (compilator != null)
                            {
                                compilator.EndSession(DateTime.Now);
                            }
                            Debug.Log("You win!");
                            AudioListener.pause = false;
                            SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }
}
