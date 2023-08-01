using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
public enum CurrentAnimation
{
    Idle = 0,
    Readying,
    Thrust,
    HitGround,
    Struggling,
    Release,
    Tantrum
}
public class BossSubBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Parameters")]
    [Range(1.0f, 50.0f)]
    public float speed = 1.0f;
    public Vector2 timeBeforeStrikeRange = new Vector2(0.0f, 1.0f);
    public float timeStunned = 5.0f;
    public float sizeReduce = 200.0f;
    public float height = 0.0f;
    public float height2 = 0.0f;
    public BossStatus status = BossStatus.Idle;

    [Header("Components")]
    public Animator animator;
    public BossMov mainBoss = null;
    public GameObject shockwavePrefab = null;
    public GameObject minionPrefab = null;
    public BossSubHitbox subHitbox;
    public CinemachineVirtualCamera bossCamera;
    public GameObject playerTarget;
    public GameObject preemptiveShadow;
    public GameObject healthBarUI;
    public CapsuleCollider capsule;
    public BoxCollider box;

    [Space(10)]
    public bool canReturn = false;
    float timeBeforeStrike = 0.0f;
    bool minionAttackType = true;
    Vector3 initialPosition = Vector3.zero;
    Vector3 thrustObjective = Vector3.zero;
    Quaternion currentRotation = Quaternion.identity;
    float timeCountStruggle = 0.0f;
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
        timeBeforeStrike = Random.Range(timeBeforeStrikeRange.x, timeBeforeStrikeRange.y);
        status = BossStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossCamera.enabled)
        {
            if (!mainBoss.isDown)
            {
                initialPosition = transform.position;
            }
            status = BossStatus.Idle;
            SwitchAnimation(CurrentAnimation.Idle);
            preemptiveShadow.SetActive(false);
            if (Vector3.Distance(transform.position, initialPosition) >= 0.01f)
            {
                transform.LookAt(initialPosition);
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, Time.deltaTime * speed / 1.5f);
            }
            else
            {
                initialPosition = transform.position;
                transform.LookAt(playerTarget.transform.position);
            }
            box.enabled = false;
            capsule.enabled = true;
            currentRotation = Quaternion.identity;
            currentTimeStrike = 0.0f;
            currentTimeStuck = 0.0f;
            currentTimeInvulnerable = 0.0f;
            timeCountStruggle = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            healthBarUI.SetActive(false);
            minionAttackType = false;
            canReturn = false;
            animator.SetFloat("speedIdle", 1.0f);
            animator.SetFloat("speedReadying", 1.0f);
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
        currentTimeStrike += Time.deltaTime;
        animator.SetFloat("speedReadying", 1.0f);

        preemptiveShadow.SetActive(true);
        preemptiveShadow.transform.position = playerTarget.transform.position;
        Vector3 positionCorrector = new Vector3(preemptiveShadow.transform.localPosition.x, height, preemptiveShadow.transform.localPosition.z);
        preemptiveShadow.transform.localPosition = positionCorrector;

        transform.LookAt(preemptiveShadow.transform.position);

        if (currentTimeStrike >= (timeBeforeStrike - 2.5f))
        {
            animator.SetFloat("speedIdle", 1.5f);
            currentAngle += Time.deltaTime * 10.0f;
            if (Time.timeScale > 0.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosition - (this.transform.forward * 2.0f), Time.deltaTime * 2.5f);
                Vector3 scale = new Vector3
                    ((Mathf.Sin(currentAngle) / sizeReduce) + preemptiveShadow.transform.localScale.x,
                    (Mathf.Sin(currentAngle) / sizeReduce) + preemptiveShadow.transform.localScale.y,
                    preemptiveShadow.transform.localScale.z);
                preemptiveShadow.transform.localScale = scale;
            }
        }
        else
        {
            animator.SetFloat("speedIdle", 1.0f);
        }

        if (currentTimeStrike >= (timeBeforeStrike - 1.2f))
        {
            SwitchAnimation(CurrentAnimation.Readying);
        }
        else
        {
            SwitchAnimation(CurrentAnimation.Idle);
        }

        if (currentTimeStrike >= timeBeforeStrike)
        {
            SwitchAnimation(CurrentAnimation.Thrust);
            status = BossStatus.Thrust;
            timeBeforeStrike = Random.Range(timeBeforeStrikeRange.x, timeBeforeStrikeRange.y);
            currentTimeStrike = 0.0f;
            currentAngle = 0.0f;
            box.enabled = false;
            capsule.enabled = true;
        }
    }
    private void ThrustState()
    {
        if (preemptiveShadow.activeSelf)
        {
            thrustObjective = preemptiveShadow.transform.position;
            preemptiveShadow.SetActive(false);
        }

        if (Vector3.Distance(thrustObjective, transform.position) >= 2.35f)
        {
            currentRotation = transform.rotation;
            transform.position = Vector3.MoveTowards(transform.position, thrustObjective, Time.deltaTime * speed);
        }
        else
        {
            SwitchAnimation(CurrentAnimation.HitGround);
            timeCountStruggle = 0.0f;
            SpawnShockwave();
            status = BossStatus.Stuck;
            box.enabled = true;
            capsule.enabled = false;
        }
    }
    private void StuckState()
    {
        currentTimeStuck += Time.deltaTime;

        if (currentTimeStuck >= timeStunned)
        {
            SwitchAnimation(CurrentAnimation.Release);
            if (canReturn)
            {
                if (Vector3.Distance(transform.position, initialPosition) >= 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, initialPosition, Time.deltaTime * speed / 1.5f);
                    transform.LookAt(thrustObjective);
                }
                else
                {
                    canReturn = false;
                    status = BossStatus.Invulnerable;
                    currentTimeStuck = 0.0f;
                    transform.rotation = Quaternion.identity;
                    currentRotation = Quaternion.identity;
                    box.enabled = false;
                    capsule.enabled = true;
                }
            }
        }
        else
        {
            SwitchAnimation(CurrentAnimation.Struggling);
            transform.rotation = Quaternion.Lerp(currentRotation, Quaternion.LookRotation(Vector3.down, Vector3.forward), timeCountStruggle * 0.65f);
            timeCountStruggle += Time.deltaTime;
        }
    }
    private void InvulnerableState()
    {
        currentTimeInvulnerable += Time.deltaTime;
        animator.SetFloat("speedReadying", 1.5f);
        if (numberThrust >= 3)
        {
            SwitchAnimation(CurrentAnimation.Idle);
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        if (currentTimeInvulnerable <= 2.0f)
        {
            SwitchAnimation(CurrentAnimation.Tantrum);
            thrustObjective = playerTarget.transform.position;
            transform.LookAt(thrustObjective);
        }
        else
        {
            switch (invulnerablePhase)
            {
                case InvulnerablePhase.Returning:
                    {
                        SwitchAnimation(CurrentAnimation.Thrust);
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
                        SwitchAnimation(CurrentAnimation.Readying);
                        if (Vector3.Distance(transform.position, initialPosition - (this.transform.forward * 2.0f)) >= 0.01f)
                        {
                            preemptiveShadow.SetActive(true);
                            preemptiveShadow.transform.position = playerTarget.transform.position;
                            Vector3 positionCorrector = new Vector3(preemptiveShadow.transform.localPosition.x, height2, preemptiveShadow.transform.localPosition.z);
                            preemptiveShadow.transform.localPosition = positionCorrector;

                            thrustObjective = preemptiveShadow.transform.position;
                            transform.LookAt(thrustObjective);
                            transform.position = Vector3.MoveTowards(transform.position, initialPosition - (this.transform.forward * 2.0f), Time.deltaTime * 3.0f);
                        }
                        else
                        {
                            invulnerablePhase = InvulnerablePhase.Thrusting;
                        }
                    }
                    break;
                case InvulnerablePhase.Thrusting:
                    {
                        SwitchAnimation(CurrentAnimation.Thrust);
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
        if (numberThrust >= 2)
        {
            SwitchAnimation(CurrentAnimation.Idle);
            status = BossStatus.Idle;
            currentTimeInvulnerable = 0.0f;
            numberThrust = 0;
            invulnerablePhase = InvulnerablePhase.Charging;
            preemptiveShadow.SetActive(false);
            minionAttackType = !minionAttackType;
            return;
        }

        SwitchAnimation(CurrentAnimation.Tantrum);
        thrustObjective = playerTarget.transform.position;
        transform.LookAt(thrustObjective);

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

    void SwitchAnimation(CurrentAnimation animation)
    {
        animator.SetInteger("currentAnimation", (int)animation);
    }
}
