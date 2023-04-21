using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum BossStatus
{
    Idle,
    Thrust,
    Stuck,
    Invulnerable
}
public enum InvulnerablePhase
{
    Returning,
    Charging,
    Thrusting
}
public class BossSubBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Parameters")]
    [Range(1.0f, 50.0f)]
    public float speed = 1.0f;
    public float timeBeforeStrike = 0.0f;
    public float timeStunned = 5.0f;
    public BossStatus status = BossStatus.Idle;

    [Header("Components")]
    public GameObject shockwavePrefab = null;
    public GameObject minionPrefab = null;
    public BossSubHitbox subHitbox;
    public CinemachineVirtualCamera bossCamera;
    public GameObject playerTarget;
    public GameObject preemptiveShadow;
    public GameObject healthBarUI;

    bool minionAttackType = true;
    Vector3 initialPosition = Vector3.zero;
    Vector3 thrustObjective = Vector3.zero;
    float currentAngle = 0.0f;
    float currentTimeStrike = 0.0f;
    float currentTimeStuck = 0.0f;
    float currentTimeInvulnerable = 0.0f;
    float timeBetweenBirds = 0.5f;
    int numberThrust = 0;
    InvulnerablePhase invulnerablePhase = InvulnerablePhase.Thrusting;
    void Start()
    {
        initialPosition = transform.position;
        timeBeforeStrike = Random.Range(4.0f, 8.0f);
        status = BossStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossCamera.enabled)
        {
            initialPosition = transform.position;
            status = BossStatus.Idle;
            preemptiveShadow.SetActive(false);
            if (Vector3.Distance(transform.position, initialPosition) >= 0.01f)
            {
                transform.LookAt(initialPosition);
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, Time.deltaTime * speed / 1.5f);
            }
            else
            {
                transform.LookAt(playerTarget.transform.position);
            }
            currentTimeStrike = 0.0f;
            currentTimeStuck = 0.0f;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            healthBarUI.SetActive(false);
            minionAttackType = false;
            return;
        }

        healthBarUI.SetActive(true);
        switch (status)
        {
            case BossStatus.Idle:
                {
                    IdleState();
                }
                break;
            case BossStatus.Thrust:
                {
                    ThrustState();
                }
                break;
            case BossStatus.Stuck:
                {
                    StuckState();
                }
                break;
            case BossStatus.Invulnerable:
                {
                    if (minionAttackType)
                    {
                        MinionState();
                    }
                    else
                    {
                        InvulnerableState();
                    }
                }
                break;
            default:
                break;
        }
    }

    private void IdleState()
    {
        preemptiveShadow.SetActive(true);
        currentTimeStrike += Time.deltaTime;
        gameObject.transform.LookAt(preemptiveShadow.transform.position);
        preemptiveShadow.transform.position = playerTarget.transform.position;
        Vector3 positionCorrector = new Vector3(preemptiveShadow.transform.localPosition.x, -0.025f, preemptiveShadow.transform.localPosition.z);
        preemptiveShadow.transform.localPosition = positionCorrector;

        if (currentTimeStrike >= (timeBeforeStrike - 2.0f))
        {
            currentAngle += Time.deltaTime * 10.0f;
            if (Time.timeScale > 0.0f)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, initialPosition - (this.transform.forward * 2.0f), Time.deltaTime * 2.5f);
                Vector3 scale = new Vector3
                    ((Mathf.Sin(currentAngle) / 200) + preemptiveShadow.transform.localScale.x,
                    preemptiveShadow.transform.localScale.y,
                    (Mathf.Sin(currentAngle) / 200) + preemptiveShadow.transform.localScale.z);
                preemptiveShadow.transform.localScale = scale;
            }
        }

        if (currentTimeStrike >= timeBeforeStrike)
        {
            status = BossStatus.Thrust;
            timeBeforeStrike = Random.Range(4.0f, 8.0f);
            currentTimeStrike = 0.0f;
            currentAngle = 0.0f;
        }
    }
    private void ThrustState()
    {
        if (preemptiveShadow.activeSelf)
        {
            thrustObjective = preemptiveShadow.transform.position;
            preemptiveShadow.SetActive(false);
        }

        if (Vector3.Distance(thrustObjective, gameObject.transform.position) >= 2.35f)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, thrustObjective, Time.deltaTime * speed);
        }
        else
        {
            SpawnShockwave();
            status = BossStatus.Stuck;
        }
    }
    private void StuckState()
    {
        currentTimeStuck += Time.deltaTime;

        if (currentTimeStuck >= timeStunned)
        {
            if (Vector3.Distance(transform.position, initialPosition) >= 0.01f)
            {
                transform.LookAt(initialPosition);
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, Time.deltaTime * speed / 1.5f);
            }
            else
            {
                status = BossStatus.Invulnerable;
                currentTimeStuck = 0.0f;
                transform.rotation = Quaternion.identity;
            }
        }
    }
    private void InvulnerableState()
    {
        currentTimeInvulnerable += Time.deltaTime;
        if (numberThrust >= 3)
        {
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        if (currentTimeInvulnerable <= 1.5f)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 400.0f);
            thrustObjective = playerTarget.transform.position;
        }
        else
        {
            switch (invulnerablePhase)
            {
                case InvulnerablePhase.Returning:
                    {
                        if (Vector3.Distance(transform.position, initialPosition) >= 0.01f)
                        {
                            transform.LookAt(initialPosition);
                            transform.position = Vector3.MoveTowards(transform.position, initialPosition, Time.deltaTime * speed);
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Charging;
                            numberThrust++;
                        }
                    }
                    break;
                case InvulnerablePhase.Charging:
                    {
                        if (Vector3.Distance(transform.position, initialPosition - (this.transform.forward * 2.0f)) >= 0.01f)
                        {
                            thrustObjective = playerTarget.transform.position;
                            transform.LookAt(thrustObjective);
                            transform.position = Vector3.MoveTowards(transform.position, initialPosition - (this.transform.forward * 2.0f), Time.deltaTime * 3.0f);
                            preemptiveShadow.SetActive(true);
                            preemptiveShadow.transform.position = playerTarget.transform.position;
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Thrusting;
                        }
                    }
                    break;
                case InvulnerablePhase.Thrusting:
                    {
                        if (Vector3.Distance(thrustObjective, transform.position) >= 2.25f)
                        {
                            transform.LookAt(thrustObjective);
                            transform.position = Vector3.MoveTowards(transform.position, thrustObjective, Time.deltaTime * speed);
                            preemptiveShadow.SetActive(false);
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Returning;
                            SpawnShockwave();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void SpawnShockwave()
    {
        GameObject instance = Instantiate(shockwavePrefab, transform.position + shockwavePrefab.transform.position, shockwavePrefab.transform.rotation);
        Vector3 scale = new Vector3(instance.transform.localScale.x, instance.transform.localScale.y, 0.3f);
        instance.transform.localScale = scale;
        ShockwaveBehaviour behaviour = instance.GetComponent<ShockwaveBehaviour>();
        if (behaviour != null)
        {
            behaviour.lifeTimeTotal = 1.0f;
            behaviour.expansionRate = 1.5f;
        }
    }

    private void MinionState()
    {
        currentTimeInvulnerable += Time.deltaTime;
        if (numberThrust >= 3)
        {
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        transform.Rotate(Vector3.up, Time.deltaTime * 400.0f);
        thrustObjective = playerTarget.transform.position;

        if (currentTimeInvulnerable >= 1.5f)
        {
            if (currentTimeInvulnerable >= 1.5f + timeBetweenBirds)
            {
                currentTimeInvulnerable = 1.5f;
                SpawnMinion();
                numberThrust++;
            }
        }
    }

    void SpawnMinion()
    {
        GameObject bird = Instantiate(minionPrefab, transform.position + new Vector3(1.0f, 0.0f), Quaternion.identity);
        bird.transform.LookAt(playerTarget.transform.position);
        bird.GetComponent<MinionBehaviour>().maxVelocity = 9.0f;
        bird = Instantiate(minionPrefab, transform.position + new Vector3(-1.0f, 0.0f), Quaternion.identity);
        bird.transform.LookAt(playerTarget.transform.position);
        bird.GetComponent<MinionBehaviour>().maxVelocity = 9.0f;
    }
}
