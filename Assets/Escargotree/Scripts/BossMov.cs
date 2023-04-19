using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMov : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Properties")]
    public bool attacking;
    public int LegToDestroy = 3;
    [Range(0.1f, 5.0f)]
    public float divider = 3.0f;

    [Header("Components")]
    public GameObject legFrontLeft;
    public GameObject legFrontRight;
    public GameObject legBackLeft;
    public GameObject legBackRight;

    [Space(10)]
    public BossHitbox frontRightHitbox;
    public BossHitbox backLeftHitbox;
    public BossHitbox backRightHitbox;

    [Space(10)]
    public GameObject collisionAreaFrontLeft;
    public GameObject collisionAreaBackLeft;
    public GameObject collisionAreaFrontRight;
    public GameObject collisionAreaBackRight;

    [Space(10)]
    public GameObject shockwavePrefab;
    public Animator animator;
    public GameObject canvas;
    public Slider healthBar;
    
    Quaternion legFrontLeftInitialRot= Quaternion.identity;
    Quaternion legFrontRightInitialRot = Quaternion.identity;
    Quaternion legBackLeftInitialRot = Quaternion.identity;
    Quaternion legBackRightInitialRot = Quaternion.identity;

    int legsDestroyed = 0;
    int isAttackingHash;
    int maxLives;
    void Start()
    {
        isAttackingHash = Animator.StringToHash("isAttacking");

        legBackLeftInitialRot = legBackLeft.transform.localRotation;
        legFrontLeftInitialRot = legFrontLeft.transform.localRotation;
        legFrontRightInitialRot = legFrontRight.transform.localRotation;
        legBackRightInitialRot = legBackRight.transform.localRotation;

        maxLives = frontRightHitbox.maxLives + backLeftHitbox.maxLives + backRightHitbox.maxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            attacking = !attacking;
            animator.SetBool(isAttackingHash, attacking);
        }

        if (legsDestroyed < 3 && attacking)
        {
            if (canvas != null)
            {
                canvas.SetActive(true);
            }

            if (healthBar != null)
            {
                healthBar.value = maxLives - frontRightHitbox.currentHits - backLeftHitbox.currentHits - backRightHitbox.currentHits;
            }

            legsDestroyed = animator.GetInteger("legsDestroyed");
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

            if (Time.timeScale > 0)
            {
                Vector3 scale = new Vector3(
                    Quaternion.Angle(legFrontRightInitialRot, legFrontRight.transform.localRotation) / divider,
                    collisionAreaFrontRight.transform.localScale.y,
                    Quaternion.Angle(legFrontRightInitialRot, legFrontRight.transform.localRotation) / divider
                    );
                collisionAreaFrontRight.transform.localScale = scale;

                scale = new Vector3(
                    Quaternion.Angle(legFrontLeftInitialRot, legFrontLeft.transform.localRotation) / divider,
                    collisionAreaFrontLeft.transform.localScale.y,
                    Quaternion.Angle(legFrontLeftInitialRot, legFrontLeft.transform.localRotation) / divider
                    );
                collisionAreaFrontLeft.transform.localScale = scale;

                scale = new Vector3(
                    Quaternion.Angle(legBackRightInitialRot, legBackRight.transform.localRotation) / divider,
                    collisionAreaBackRight.transform.localScale.y,
                    Quaternion.Angle(legBackRightInitialRot, legBackRight.transform.localRotation) / divider
                    );
                collisionAreaBackRight.transform.localScale = scale;

                scale = new Vector3(
                    Quaternion.Angle(legBackLeftInitialRot, legBackLeft.transform.localRotation) / divider,
                    collisionAreaBackLeft.transform.localScale.y,
                    Quaternion.Angle(legBackLeftInitialRot, legBackLeft.transform.localRotation) / divider
                    );
                collisionAreaBackLeft.transform.localScale = scale;
            }
        }
        else
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
            attacking = false;
            animator.SetBool(isAttackingHash, false);
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
        collisionAreaBackRight.SetActive(true);
        collisionAreaFrontRight.SetActive(false);
        collisionAreaBackLeft.SetActive(false);
    }

    void SpawnShockwave()
    {
        if (!collisionAreaBackLeft.activeSelf)
        {
            Instantiate(shockwavePrefab, collisionAreaBackLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        }

        if (!collisionAreaBackRight.activeSelf)
        {
            Instantiate(shockwavePrefab, collisionAreaBackRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        }

        if (!collisionAreaFrontLeft.activeSelf)
        {
            Instantiate(shockwavePrefab, collisionAreaFrontLeft.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        }

        if (!collisionAreaFrontRight.activeSelf)
        {
            Instantiate(shockwavePrefab, collisionAreaFrontRight.transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        }
    }

}
