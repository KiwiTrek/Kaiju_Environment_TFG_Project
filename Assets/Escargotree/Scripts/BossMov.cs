using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMov : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Properties")]
    public bool isDown = false;
    public bool attacking;
    public int LegToDestroy = 3;
    [Range(0.1f, 30.0f)]
    public float divider = 3.0f;

    [Header("Components")]
    public GameObject legFrontLeft;
    public GameObject legFrontRight;
    public GameObject legBackLeft;
    public GameObject legBackRight;

    [Space(10)]
    public AudioSource legFrontLeftAudio;
    public AudioSource legFrontRightAudio;
    public AudioSource legBackLeftAudio;
    public AudioSource legBackRightAudio;

    [Space(10)]
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

    void Start()
    {
        isAttackingHash = Animator.StringToHash("isAttacking");
        maxLives = frontRightHitbox.maxLives + backLeftHitbox.maxLives + backRightHitbox.maxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            attacking = !attacking;
            animator.SetBool(isAttackingHash, attacking);
            if (legsDestroyed < 3 && attacking == true && canvas != null)
            {
                canvas.SetActive(true);
            }
        }

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
            if (Time.timeScale > 0)
            {

                Vector3 scale = new Vector3(
                    legFrontLeft.transform.position.y / (divider),
                    legFrontLeft.transform.position.y / (divider),
                    collisionAreaFrontLeft.transform.localScale.z
                    );
                collisionAreaFrontLeft.transform.localScale = scale;

                scale = new Vector3(
                    legFrontRight.transform.position.y / (divider),
                    legFrontRight.transform.position.y / (divider),
                    collisionAreaFrontRight.transform.localScale.z
                    );
                collisionAreaFrontRight.transform.localScale = scale;

                scale = new Vector3(
                    legBackRight.transform.position.y / (divider),
                    legBackRight.transform.position.y / (divider),
                    collisionAreaBackRight.transform.localScale.z
                    );
                collisionAreaBackRight.transform.localScale = scale;

                scale = new Vector3(
                    legBackLeft.transform.position.y / (divider),
                    legBackLeft.transform.position.y / (divider),
                    collisionAreaBackLeft.transform.localScale.z
                    );
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
            legsDestroyed = newValue;
            animator.SetInteger("legsDestroyed", legsDestroyed);
            float legSpeed = 0.48f + legsDestroyed * 0.18f;
            finalHeightMultiplier = 0.26f + legsDestroyed * 0.075f;
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

    void SwitchActiveColliders()
    {
        if (collisionAreaBackLeft.activeSelf)
        {
            collisionAreaBackLeft.SetActive(false);
        }
        else
        {
            collisionAreaBackLeft.SetActive(true);
        }

        if (collisionAreaBackRight.activeSelf)
        {
            collisionAreaBackRight.SetActive(false);
        }
        else
        {
            collisionAreaBackRight.SetActive(true);
        }

        if (collisionAreaFrontLeft.activeSelf)
        {
            collisionAreaFrontLeft.SetActive(false);
        }
        else
        {
            collisionAreaFrontLeft.SetActive(true);
        }

        if (collisionAreaFrontRight.activeSelf)
        {
            collisionAreaFrontRight.SetActive(false);
        }
        else
        {
            collisionAreaFrontRight.SetActive(true);
        }
    }
    void ResetAttackPattern()
    {
        collisionAreaFrontLeft.SetActive(true);
        collisionAreaFrontRight.SetActive(false);
        collisionAreaBackRight.SetActive(true);
        collisionAreaBackLeft.SetActive(false);
    }

    void SpawnShockwave()
    {
        if (!collisionAreaBackLeft.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaBackLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaBackLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            legBackLeftAudio.Play();
        }

        if (!collisionAreaBackRight.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaBackRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaBackRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            legBackRightAudio.Play();
        }

        if (!collisionAreaFrontLeft.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaFrontLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaFrontLeft.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            legFrontLeftAudio.Play();
        }

        if (!collisionAreaFrontRight.activeSelf)
        {
            GameObject wave = Instantiate(shockwavePrefab, collisionAreaFrontRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
            wave.GetComponentInChildren<ShockwaveBehaviour>().finalHeightMultiplier = finalHeightMultiplier;
            GameObject particles = Instantiate(shockwavePrefab2, collisionAreaFrontRight.transform.position + shockwavePrefab2.transform.position, shockwavePrefab2.transform.rotation);
            Destroy(particles, 1.5f);
            legFrontRightAudio.Play();
        }
    }

    void SwitchDown()
    {
        isDown = !isDown;
    }
}
