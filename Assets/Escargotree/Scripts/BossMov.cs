using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundTypeEscargotree
{
    LegRising,
    AboutToHit,
    Explosion,
    LegDeath,
    WoodCreak
}

public class BossMov : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Properties")]
    public bool isDown = false;
    public bool attacking;
    public int LegToDestroy = 3;
    [Range(0.1f, 30.0f)]
    public float divider = 3.0f;
    [Space]
    [Header("Audio Clips")]
    public AudioClip[] legRising = null;
    public AudioClip[] aboutToHit = null;
    public AudioClip[] explosion = null;
    public AudioClip[] legDeath = null;
    public AudioClip woodCreakSingle = null;

    [Header("Components")]
    public GameObject legFrontLeft;
    public GameObject legFrontRight;
    public GameObject legBackLeft;
    public GameObject legBackRight;

    [Space(10)]
    public AudioSource cutsceneAudio;
    public AudioSource mouthSound;
    public AudioSource legFrontLeftAudio;
    public AudioSource legFrontRightAudio;
    public AudioSource legBackLeftAudio;
    public AudioSource legBackRightAudio;

    [Space(10)]
    public int hits = 0;
    public BossHitbox frontRightHitbox;
    public BossHitbox backLeftHitbox;
    public BossHitbox backRightHitbox;

    [Space(10)]
    public GameObject collisionAreaFrontLeft;
    public GameObject collisionAreaFrontRight;
    public GameObject collisionAreaBackLeft;
    public GameObject collisionAreaBackRight;

    [Space(10)]
    public List<GameObject> spikes = null;
    public GameObject shockwavePrefab;
    public GameObject shockwavePrefab2;
    public float finalHeightMultiplier = 0.6666666667f;
    public Animator animator;
    public GameObject canvas;
    public Slider healthBar;

    public int legsDestroyed = 0;
    int isAttackingHash;
    int maxLives;

    [Space(10)]
    public bool switchToSecondCam = false;
    public GameObject camIntro2 = null;
    CinemachineVirtualCamera camIntroScript2 = null;
    CinemachineBasicMultiChannelPerlin camIntroNoise2 = null;
    Vector3 angleFinal = new(0.0f,0.0f,0.0f);

    void Start()
    {
        isAttackingHash = Animator.StringToHash("isAttacking");
        maxLives = frontRightHitbox.maxLives + backLeftHitbox.maxLives + backRightHitbox.maxLives;

        if (camIntro2 != null)
        {
            camIntroScript2 = camIntro2.GetComponent<CinemachineVirtualCamera>();
            camIntroNoise2 = camIntroScript2.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        }

        frontRightHitbox.currentHits = hits;
        backLeftHitbox.currentHits = hits;
        backRightHitbox.currentHits = hits;

        Vector3 euler = transform.eulerAngles;
        euler.y -= 180.0f;
        angleFinal = euler;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown (KeyCode.F2))
        {
            backLeftHitbox.currentHits += 1;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            backRightHitbox.currentHits += 1;
        }
#endif

        if (legsDestroyed < 3 && attacking)
        {
            foreach (GameObject spike in spikes)
            {
                spike.SetActive(true);
            }

            if (healthBar != null)
            {
                healthBar.value = maxLives - frontRightHitbox.currentHits - backLeftHitbox.currentHits - backRightHitbox.currentHits;
            }

            legsDestroyed = animator.GetInteger("legsDestroyed");
            if (Time.timeScale > 0 && GameplayDirector.cutsceneMode == CutsceneType.None)
            {
                Vector3 scale = new(
                    legFrontLeft.transform.position.y / (divider),
                    legFrontLeft.transform.position.y / (divider),
                    collisionAreaFrontLeft.transform.localScale.z
                    );
                collisionAreaFrontLeft.transform.localScale = scale;

                scale = new(
                    legFrontRight.transform.position.y / (divider),
                    legFrontRight.transform.position.y / (divider),
                    collisionAreaFrontRight.transform.localScale.z
                    );
                collisionAreaFrontRight.transform.localScale = scale;

                scale = new(
                    legBackRight.transform.position.y / (divider),
                    legBackRight.transform.position.y / (divider),
                    collisionAreaBackRight.transform.localScale.z
                    );
                collisionAreaBackRight.transform.localScale = scale;

                scale = new(
                    legBackLeft.transform.position.y / (divider),
                    legBackLeft.transform.position.y / (divider),
                    collisionAreaBackLeft.transform.localScale.z
                    );
                collisionAreaBackLeft.transform.localScale = scale;
            }
            else
            {
                Vector3 scale = new(0.01f, 0.01f, 0.0025f);
                collisionAreaFrontLeft.transform.localScale = scale;
                collisionAreaFrontRight.transform.localScale = scale;
                collisionAreaBackRight.transform.localScale = scale;
                collisionAreaBackLeft.transform.localScale = scale;
            }

            int newValue = 0;
            if (frontRightHitbox.currentHits >= frontRightHitbox.maxLives)
            {
                newValue++;
            }
            if (backLeftHitbox.currentHits >= frontRightHitbox.maxLives)
            {
                newValue++;
            }
            if (backRightHitbox.currentHits >= frontRightHitbox.maxLives)
            {
                newValue++;
            }

            if (newValue != legsDestroyed)
            {
                PlaySoundMouth(SoundTypeEscargotree.LegDeath);
            }
            legsDestroyed = newValue;
            animator.SetInteger("legsDestroyed", legsDestroyed);
            float legSpeed = 0.48f + legsDestroyed * 0.18f;
            finalHeightMultiplier = 0.08f + legsDestroyed * 0.03f;
            animator.SetFloat("legSpeed", legSpeed);
        }
        else
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
            attacking = false;
            animator.SetBool(isAttackingHash, false);
            foreach (GameObject spike in spikes)
            {
                spike.SetActive(false);
            }
            collisionAreaBackLeft.SetActive(false);
            collisionAreaBackRight.SetActive(false);
            collisionAreaFrontLeft.SetActive(false);
            collisionAreaFrontRight.SetActive(false);
        }
    }

    public void SwitchActiveColliders()
    {
        float legSpeed = 0.48f + legsDestroyed * 0.18f;
        if (collisionAreaBackLeft.activeSelf)
        {
            collisionAreaBackLeft.SetActive(false);
        }
        else
        {
            collisionAreaBackLeft.SetActive(true);
            PlaySoundLeg(legBackLeftAudio, SoundTypeEscargotree.LegRising, 3.5f - legSpeed);
        }

        if (collisionAreaBackRight.activeSelf)
        {
            collisionAreaBackRight.SetActive(false);
        }
        else
        {
            collisionAreaBackRight.SetActive(true);
            PlaySoundLeg(legBackRightAudio, SoundTypeEscargotree.LegRising, 3.5f - legSpeed);
        }

        if (collisionAreaFrontLeft.activeSelf)
        {
            collisionAreaFrontLeft.SetActive(false);
        }
        else
        {
            collisionAreaFrontLeft.SetActive(true);
            PlaySoundLeg(legFrontLeftAudio, SoundTypeEscargotree.LegRising, 3.5f - legSpeed);
        }

        if (collisionAreaFrontRight.activeSelf)
        {
            collisionAreaFrontRight.SetActive(false);
        }
        else
        {
            collisionAreaFrontRight.SetActive(true);
            PlaySoundLeg(legFrontRightAudio, SoundTypeEscargotree.LegRising, 3.5f - legSpeed);
        }
    }
    public void ResetAttackPattern()
    {
        collisionAreaFrontLeft.SetActive(true);
        collisionAreaFrontRight.SetActive(false);
        collisionAreaBackRight.SetActive(true);
        collisionAreaBackLeft.SetActive(false);
    }

    public void SetAllCollidersToTrue()
    {
        collisionAreaFrontLeft.SetActive(true);
        collisionAreaFrontRight.SetActive(true);
        collisionAreaBackRight.SetActive(true);
        collisionAreaBackLeft.SetActive(true);
    }

    public void SpawnShockwave()
    {
        if (!collisionAreaBackLeft.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaBackLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaBackLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            PlaySoundLeg(legBackLeftAudio, SoundTypeEscargotree.Explosion);
        }

        if (!collisionAreaBackRight.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaBackRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaBackRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            PlaySoundLeg(legBackRightAudio, SoundTypeEscargotree.Explosion);
        }

        if (!collisionAreaFrontLeft.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaFrontLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaFrontLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            PlaySoundLeg(legFrontLeftAudio, SoundTypeEscargotree.Explosion);
        }

        if (!collisionAreaFrontRight.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaFrontRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaFrontRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            PlaySoundLeg(legFrontRightAudio, SoundTypeEscargotree.Explosion);
        }
    }
    public void SpawnShockwaveLeg()
    {
        GameObject wave = Instantiate(shockwavePrefab, legBackLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
        GameObject particles = Instantiate(shockwavePrefab2, legBackLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
        Destroy(particles, 1.5f);
        PlaySoundLeg(legBackLeftAudio, SoundTypeEscargotree.Explosion);

        wave = Instantiate(shockwavePrefab, legBackRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
        particles = Instantiate(shockwavePrefab2, legBackRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
        Destroy(particles, 1.5f);
        PlaySoundLeg(legBackRightAudio, SoundTypeEscargotree.Explosion);

        wave = Instantiate(shockwavePrefab, legFrontLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
        particles = Instantiate(shockwavePrefab2, legFrontLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
        Destroy(particles, 1.5f);
        PlaySoundLeg(legFrontLeftAudio, SoundTypeEscargotree.Explosion);

        wave = Instantiate(shockwavePrefab, legFrontRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
        particles = Instantiate(shockwavePrefab2, legFrontRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
        Destroy(particles, 1.5f);
        PlaySoundLeg(legFrontRightAudio, SoundTypeEscargotree.Explosion);
    }

    public void SwitchDown()
    {
        isDown = !isDown;
    }

    public void PlaySoundMouth(SoundTypeEscargotree type)
    {
        mouthSound.pitch = Random.Range(0.95f, 1.1f);
        switch (type)
        {
            case SoundTypeEscargotree.LegRising:
                mouthSound.pitch = Random.Range(0.90f, 1.1f);
                mouthSound.PlayOneShot(legRising[Random.Range(0, legRising.Length)]);
                break;
            case SoundTypeEscargotree.AboutToHit:
                mouthSound.PlayOneShot(aboutToHit[Random.Range(0, aboutToHit.Length)]);
                break;
            case SoundTypeEscargotree.Explosion:
                mouthSound.pitch = Random.Range(0.90f, 1.1f);
                mouthSound.PlayOneShot(explosion[Random.Range(0, explosion.Length)]);
                break;
            case SoundTypeEscargotree.LegDeath:
                mouthSound.PlayOneShot(legDeath[Random.Range(0, legDeath.Length)]);
                if (camIntroNoise2 != null) camIntroNoise2.m_AmplitudeGain = 5.0f;
                break;
            default:
                break;
        }
    }
    public void PlaySoundLeg(AudioSource source, SoundTypeEscargotree type, float delayTime = 0.0f)
    {
        source.pitch = Random.Range(0.90f, 1.1f);
        switch (type)
        {
            case SoundTypeEscargotree.LegRising:
                source.clip = legRising[Random.Range(0, legRising.Length)];
                break;
            case SoundTypeEscargotree.AboutToHit:
                source.pitch = Random.Range(0.95f, 1.1f);
                source.clip = aboutToHit[Random.Range(0, aboutToHit.Length)];
                break;
            case SoundTypeEscargotree.Explosion:
                source.clip = explosion[Random.Range(0, explosion.Length)];
                break;
            case SoundTypeEscargotree.LegDeath:
                source.pitch = Random.Range(0.95f, 1.1f);
                source.clip = legDeath[Random.Range(0, legDeath.Length)];
                break;
            default:
                break;
        }
        source.PlayDelayed(delayTime);
    }

    public void PlaySoundCutscene(SoundTypeEscargotree type)
    {
        cutsceneAudio.pitch = Random.Range(0.95f, 1.1f);
        switch (type)
        {
            case SoundTypeEscargotree.LegRising:
                cutsceneAudio.pitch = Random.Range(0.90f, 1.1f);
                cutsceneAudio.PlayOneShot(legRising[Random.Range(0, legRising.Length)]);
                break;
            case SoundTypeEscargotree.AboutToHit:
                cutsceneAudio.PlayOneShot(aboutToHit[Random.Range(0, aboutToHit.Length)]);
                break;
            case SoundTypeEscargotree.Explosion:
                cutsceneAudio.pitch = Random.Range(0.90f, 1.1f);
                cutsceneAudio.PlayOneShot(explosion[Random.Range(0, explosion.Length)]);
                break;
            case SoundTypeEscargotree.LegDeath:
                cutsceneAudio.PlayOneShot(legDeath[Random.Range(0, legDeath.Length)]);
                if (camIntroNoise2 != null) camIntroNoise2.m_AmplitudeGain = 5.0f;
                break;
            case SoundTypeEscargotree.WoodCreak:
                cutsceneAudio.PlayOneShot(woodCreakSingle);
                break;
            default:
                break;
        }
    }

    public void SwitchToSecondCam()
    {
        if (!switchToSecondCam) switchToSecondCam = true;
        else switchToSecondCam = false;

        Debug.Log("SwitchCam: " + switchToSecondCam);
    }
}
